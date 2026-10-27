using StockFlow.Application.Validations;
using StockFlow.Application.Interfaces.Repositories.Scheduling;
using StockFlow.Application.Interfaces.Scheduling;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Mappings.Scheduling;
using StockFlow.Application.Validations.Scheduling;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Scheduling;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Scheduling;

namespace StockFlow.Application.Services.Scheduling
{
    public class ScheduleService(IScheduleRepository repository) : IScheduleService
    {
        public async Task<List<ShiftTemplateDto>> GetTemplatesAsync() =>
            (await repository.GetTemplatesAsync())
                .OrderBy(t => t.Role)
                .ThenBy(t => t.StartTime)
                .ThenBy(t => t.Name)
                .Select(t => t.ToDto())
                .ToList();

        public async Task<ShiftTemplateDto> CreateTemplateAsync(SaveShiftTemplateRequest request)
        {
            request.Validate();
            var template = new ShiftTemplate { Id = Guid.NewGuid(), Name = request.Name.Trim() };
            Apply(template, request);
            await repository.AddTemplateAsync(template);
            return template.ToDto();
        }

        public async Task<ShiftTemplateDto> UpdateTemplateAsync(Guid id, SaveShiftTemplateRequest request)
        {
            request.Validate();
            var template = await repository.GetTemplateAsync(id) ?? throw new KeyNotFoundException("Shift not found.");
            Apply(template, request);
            template.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return template.ToDto();
        }

        public async Task DeleteTemplateAsync(Guid id)
        {
            var template = await repository.GetTemplateAsync(id) ?? throw new KeyNotFoundException("Shift not found.");
            await repository.RemoveTemplateAsync(template);
        }

        public async Task<List<StaffMemberDto>> GetStaffAsync() =>
            (await repository.GetStaffAsync())
                .OrderBy(e => e.Role)
                .ThenBy(e => e.EmployeeId, StringComparer.Ordinal)
                .Select(e => e.ToStaffMemberDto())
                .ToList();

        public async Task<StaffMemberDto> SaveStaffAsync(string employeeNumber, SaveStaffRequest request)
        {
            request.Validate();
            var employee = await repository.GetStaffMemberAsync(employeeNumber?.Trim() ?? "")
                ?? throw new KeyNotFoundException("Employee not found.");

            employee.ContractHours = request.ContractHours;
            repository.ReplaceAvailability(employee, (request.Availability ?? [])
                .Select(a => new EmployeeAvailability
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    Day = a.Day,
                    AvailableFrom = a.AvailableFrom,
                    AvailableUntil = a.AvailableUntil,
                })
                .ToList());
            employee.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return employee.ToStaffMemberDto();
        }

        public async Task<List<ScheduleWeekSummaryDto>> GetWeeksAsync(DateOnly today, bool canSeeDrafts)
        {
            var first = ScheduleRules.WeekStart(today);
            var last = first.AddDays(7 * ScheduleRules.WeeksAhead);
            var weeks = (await repository.GetWeeksAsync(first, last)).ToDictionary(w => w.WeekStart);

            return Enumerable.Range(0, ScheduleRules.WeeksAhead + 1)
                .Select(i => first.AddDays(7 * i))
                .Select(start =>
                {
                    if (!weeks.TryGetValue(start, out var week))
                        return new ScheduleWeekSummaryDto { WeekStart = start };
                    bool hidden = week.Status == ScheduleWeekStatus.Draft && !canSeeDrafts;
                    return new ScheduleWeekSummaryDto
                    {
                        WeekStart = start,
                        Exists = true,
                        Status = week.Status.ToDto(),
                        Shifts = hidden ? 0 : week.Shifts.Count,
                        OpenShifts = hidden ? 0 : week.Shifts.Count(s => s.EmployeeId is null),
                    };
                })
                .ToList();
        }

        public async Task<ScheduleWeekDto> GetWeekAsync(DateOnly weekStart, bool canSeeDrafts)
        {
            var start = ScheduleRules.WeekStart(weekStart);
            var week = await repository.GetWeekAsync(start);
            return await ToWeekDtoAsync(start, week, canSeeDrafts);
        }

        public async Task<ScheduleWeekDto> GenerateWeekAsync(DateOnly weekStart, DateOnly today)
        {
            var start = RequireMonday(weekStart);
            if (start <= ScheduleRules.WeekStart(today))
                throw new ArgumentException("Only weeks after the current week can be generated.");

            var week = await repository.GetWeekAsync(start);
            if (week?.Status == ScheduleWeekStatus.Confirmed)
                throw new ArgumentException("This week is already confirmed, so it can't be generated again.");

            await FillAsync(start, week);
            return await ToWeekDtoAsync(start, await repository.GetWeekAsync(start), true);
        }

        public async Task<int> GenerateUpcomingWeeksAsync(DateOnly today)
        {
            if ((await repository.GetTemplatesAsync()).Count == 0)
                return 0;

            int created = 0;
            foreach (var start in ScheduleRules.UpcomingWeeks(today))
            {
                if (await repository.GetWeekAsync(start) is not null) continue;
                await FillAsync(start, null);
                created++;
            }
            return created;
        }

        public async Task<ScheduleWeekDto> ConfirmWeekAsync(DateOnly weekStart, Guid employeeId, DateOnly today)
        {
            var start = RequireMonday(weekStart);
            var week = await repository.GetWeekAsync(start) ?? throw new KeyNotFoundException("This week has no schedule yet.");
            if (week.Status == ScheduleWeekStatus.Confirmed)
                throw new ArgumentException("This week is already confirmed.");
            if (start.AddDays(6) < today)
                throw new ArgumentException("This week is over and can't be confirmed anymore.");

            week.Status = ScheduleWeekStatus.Confirmed;
            week.ConfirmedAt = DateTime.UtcNow;
            week.ConfirmedById = employeeId;
            week.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return await ToWeekDtoAsync(start, await repository.GetWeekAsync(start), true);
        }

        public async Task<ScheduleWeekDto> AddShiftAsync(SaveShiftRequest request, bool canEditConfirmed, DateOnly today)
        {
            Guard.NotNull(request, "Shift");
            var start = ScheduleRules.WeekStart(request.Date);
            request.Validate(start);

            var week = await repository.GetWeekAsync(start);
            if (week is null)
            {
                if (start <= ScheduleRules.WeekStart(today))
                    throw new ArgumentException("Shifts can only be added to the current or an upcoming week with a schedule.");
                week = new ScheduleWeek { Id = Guid.NewGuid(), WeekStart = start, GeneratedAt = DateTime.UtcNow };
                await repository.AddWeekAsync(week);
            }
            EnsureEditable(week, canEditConfirmed, today);

            var shift = new ScheduledShift { Id = Guid.NewGuid(), ScheduleWeekId = week.Id };
            await ApplyAsync(shift, request, week);
            week.Shifts.Add(shift);
            repository.AddShifts([shift]);
            week.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return await ToWeekDtoAsync(start, await repository.GetWeekAsync(start), true);
        }

        public async Task<ScheduleWeekDto> UpdateShiftAsync(Guid shiftId, SaveShiftRequest request, bool canEditConfirmed, DateOnly today)
        {
            Guard.NotNull(request, "Shift");
            var found = await repository.GetShiftAsync(shiftId) ?? throw new KeyNotFoundException("Shift not found.");
            var week = await repository.GetWeekAsync(found.ScheduleWeek!.WeekStart) ?? throw new KeyNotFoundException("Shift not found.");
            request.Validate(week.WeekStart);
            EnsureEditable(week, canEditConfirmed, today);

            var shift = week.Shifts.First(s => s.Id == shiftId);
            await ApplyAsync(shift, request, week);
            week.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return await ToWeekDtoAsync(week.WeekStart, await repository.GetWeekAsync(week.WeekStart), true);
        }

        public async Task<ScheduleWeekDto> DeleteShiftAsync(Guid shiftId, bool canEditConfirmed, DateOnly today)
        {
            var found = await repository.GetShiftAsync(shiftId) ?? throw new KeyNotFoundException("Shift not found.");
            var week = await repository.GetWeekAsync(found.ScheduleWeek!.WeekStart) ?? throw new KeyNotFoundException("Shift not found.");
            EnsureEditable(week, canEditConfirmed, today);

            var shift = week.Shifts.First(s => s.Id == shiftId);
            week.Shifts.Remove(shift);
            repository.RemoveShifts([shift]);
            week.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return await ToWeekDtoAsync(week.WeekStart, await repository.GetWeekAsync(week.WeekStart), true);
        }

        public async Task<List<ScheduledShiftDto>> GetMyShiftsAsync(Guid employeeId, DateOnly today)
        {
            var until = ScheduleRules.WeekStart(today).AddDays(7 * (ScheduleRules.WeeksAhead + 1) - 1);
            return (await repository.GetConfirmedShiftsAsync(employeeId, today, until))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .Select(s => s.ToDto())
                .ToList();
        }

        public async Task<List<ColleagueDto>> GetColleaguesAsync(Guid employeeId, DateOnly today)
        {
            var staff = await repository.GetStaffAsync();
            var me = staff.FirstOrDefault(e => e.Id == employeeId);
            if (me is null)
                return [];

            var until = ScheduleRules.WeekStart(today).AddDays(7 * (ScheduleRules.WeeksAhead + 1) - 1);
            var colleagues = new List<ColleagueDto>();
            foreach (var colleague in staff.Where(e => e.Role == me.Role && e.Id != me.Id).OrderBy(e => e.FullName))
                colleagues.Add(new ColleagueDto
                {
                    EmployeeId = colleague.EmployeeId,
                    FullName = colleague.FullName,
                    Role = colleague.Role.ToDto(),
                    UpcomingShifts = (await repository.GetConfirmedShiftsAsync(colleague.Id, today.AddDays(1), until))
                        .OrderBy(s => s.Date)
                        .ThenBy(s => s.StartTime)
                        .Select(s => s.ToDto())
                        .ToList(),
                });
            return colleagues;
        }

        public async Task<List<ScheduleRequestDto>> GetRequestsAsync(ScheduleRequestStatusDto? status, Guid employeeId, bool canReview)
        {
            if (status is { } value && !Enum.IsDefined(value))
                throw new ArgumentException("The status is invalid.");
            return (await repository.GetRequestsAsync(status?.ToDomain(), canReview ? null : employeeId))
                .OrderBy(r => r.Status != ScheduleRequestStatus.Pending)
                .ThenByDescending(r => r.RequestNumber)
                .Select(r => r.ToDto())
                .ToList();
        }

        public async Task<ScheduleRequestDto> CreateRequestAsync(CreateScheduleRequest request, Guid employeeId, DateOnly today)
        {
            request.Validate(today);

            var entity = new ScheduleRequest
            {
                Id = Guid.NewGuid(),
                Type = request.Type.ToDomain(),
                EmployeeId = employeeId,
                Reason = request.Reason.Trim(),
            };

            if (request.Type == ScheduleRequestTypeDto.DayOff)
            {
                var date = request.Date!.Value;
                if (await repository.HasPendingRequestAsync(employeeId, null, date))
                    throw new ArgumentException($"You already asked for {date:dd-MM-yyyy} off.");
                entity.Date = date;
            }
            else
            {
                var shift = await RequireOwnFutureShiftAsync(request.ShiftId!.Value, employeeId, today, "your shift");
                var target = await repository.GetStaffMemberAsync(request.TargetEmployeeId!.Trim())
                    ?? throw new KeyNotFoundException("Colleague not found.");
                if (target.Id == employeeId)
                    throw new ArgumentException("Pick a colleague, not yourself.");
                if (target.Role != shift.Role)
                    throw new ArgumentException($"{target.FullName} can't take over a {shift.Role} shift.");
                if (await repository.HasPendingRequestAsync(employeeId, shift.Id, null))
                    throw new ArgumentException("There already is an open request for this shift.");

                entity.ShiftId = shift.Id;
                entity.TargetEmployeeId = target.Id;

                if (request.TargetShiftId is Guid targetShiftId)
                {
                    var targetShift = await RequireOwnFutureShiftAsync(targetShiftId, target.Id, today, $"a shift of {target.FullName}");
                    if (targetShift.Role != shift.Role)
                        throw new ArgumentException($"You can only swap with a {shift.Role} shift.");
                    entity.TargetShiftId = targetShift.Id;
                }
            }

            entity.RequestNumber = await repository.GetNextRequestNumberAsync();
            await repository.AddRequestAsync(entity);
            return (await repository.GetRequestAsync(entity.Id))!.ToDto();
        }

        public async Task<ScheduleRequestDto> CancelRequestAsync(Guid id, Guid employeeId)
        {
            var request = await repository.GetRequestAsync(id) ?? throw new KeyNotFoundException("Request not found.");
            if (request.EmployeeId != employeeId)
                throw new ArgumentException("You can only cancel your own requests.");
            EnsurePending(request);

            request.Status = ScheduleRequestStatus.Cancelled;
            request.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return request.ToDto();
        }

        public async Task<ScheduleRequestDto> ApproveRequestAsync(Guid id, ReviewScheduleRequest review, Guid reviewerId, DateOnly today)
        {
            review.Validate();
            var request = await RequireReviewableAsync(id, reviewerId);

            if (request.Type == ScheduleRequestType.DayOff)
            {
                var date = request.Date!.Value;
                if (date <= today)
                    throw new ArgumentException("This day has already started, so it can't be approved anymore.");
                var week = await repository.GetWeekAsync(ScheduleRules.WeekStart(date));
                foreach (var shift in week?.Shifts.Where(s => s.EmployeeId == request.EmployeeId && s.Date == date) ?? [])
                    shift.EmployeeId = null;
            }
            else
            {
                var shift = request.Shift ?? throw new KeyNotFoundException("The shift of this request no longer exists.");
                if (shift.EmployeeId != request.EmployeeId)
                    throw new ArgumentException("This shift is no longer assigned to the employee who asked.");
                if (shift.Date <= today)
                    throw new ArgumentException("This shift has already started, so it can't be swapped anymore.");

                var targetShift = request.TargetShift;
                if (request.TargetShiftId is not null)
                {
                    if (targetShift is null)
                        throw new KeyNotFoundException("The shift to swap with no longer exists.");
                    if (targetShift.EmployeeId != request.TargetEmployeeId)
                        throw new ArgumentException("The shift to swap with is no longer assigned to the colleague.");
                    if (targetShift.Date <= today)
                        throw new ArgumentException("The shift to swap with has already started.");
                }

                await EnsureFreeAsync(request.TargetEmployeeId!.Value, shift.Date, targetShift?.Id, request.TargetEmployee?.FullName);
                if (targetShift is not null)
                    await EnsureFreeAsync(request.EmployeeId, targetShift.Date, shift.Id, request.Employee?.FullName);

                shift.EmployeeId = request.TargetEmployeeId;
                if (targetShift is not null)
                    targetShift.EmployeeId = request.EmployeeId;
            }

            Review(request, ScheduleRequestStatus.Approved, review, reviewerId);
            await repository.SaveChangesAsync();
            return (await repository.GetRequestAsync(id))!.ToDto();
        }

        public async Task<ScheduleRequestDto> RejectRequestAsync(Guid id, ReviewScheduleRequest review, Guid reviewerId)
        {
            review.Validate();
            var request = await RequireReviewableAsync(id, reviewerId);
            Review(request, ScheduleRequestStatus.Rejected, review, reviewerId);
            await repository.SaveChangesAsync();
            return (await repository.GetRequestAsync(id))!.ToDto();
        }

        private async Task FillAsync(DateOnly start, ScheduleWeek? week)
        {
            var templates = await repository.GetTemplatesAsync();
            var staff = await repository.GetStaffAsync();
            var daysOff = await repository.GetApprovedDaysOffAsync(start, start.AddDays(6));
            var shifts = ScheduleGenerator.Generate(start, templates, staff, daysOff);

            if (week is null)
            {
                week = new ScheduleWeek { Id = Guid.NewGuid(), WeekStart = start, GeneratedAt = DateTime.UtcNow };
                foreach (var shift in shifts) week.Shifts.Add(shift);
                await repository.AddWeekAsync(week);
                return;
            }

            repository.RemoveShifts(week.Shifts.ToList());
            week.Shifts.Clear();
            foreach (var shift in shifts)
            {
                shift.ScheduleWeekId = week.Id;
                week.Shifts.Add(shift);
            }
            repository.AddShifts(shifts);
            week.GeneratedAt = DateTime.UtcNow;
            week.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
        }

        private async Task ApplyAsync(ScheduledShift shift, SaveShiftRequest request, ScheduleWeek week)
        {
            shift.Date = request.Date;
            shift.StartTime = request.StartTime;
            shift.EndTime = request.EndTime;
            shift.Role = request.Role.ToDomain();

            if (string.IsNullOrWhiteSpace(request.EmployeeId))
            {
                shift.EmployeeId = null;
                shift.Employee = null;
                return;
            }

            var employee = await repository.GetStaffMemberAsync(request.EmployeeId.Trim())
                ?? throw new KeyNotFoundException("Employee not found.");
            if (employee.Role != shift.Role)
                throw new ArgumentException($"{employee.FullName} is a {employee.Role}, this shift needs a {shift.Role}.");
            if (week.Shifts.Any(s => s.Id != shift.Id && s.EmployeeId == employee.Id && s.Date == shift.Date))
                throw new ArgumentException($"{employee.FullName} already works on {shift.Date:dd-MM-yyyy}.");
            if ((await repository.GetApprovedDaysOffAsync(shift.Date, shift.Date)).Contains((employee.Id, shift.Date)))
                throw new ArgumentException($"{employee.FullName} has a day off on {shift.Date:dd-MM-yyyy}.");

            shift.EmployeeId = employee.Id;
            shift.Employee = employee;
        }

        private async Task EnsureFreeAsync(Guid employeeId, DateOnly date, Guid? exceptShiftId, string? name)
        {
            var week = await repository.GetWeekAsync(ScheduleRules.WeekStart(date));
            if (week?.Shifts.Any(s => s.EmployeeId == employeeId && s.Date == date && s.Id != exceptShiftId) == true)
                throw new ArgumentException($"{name ?? "The employee"} already works on {date:dd-MM-yyyy}.");
            if ((await repository.GetApprovedDaysOffAsync(date, date)).Contains((employeeId, date)))
                throw new ArgumentException($"{name ?? "The employee"} has a day off on {date:dd-MM-yyyy}.");
        }

        private async Task<ScheduledShift> RequireOwnFutureShiftAsync(Guid shiftId, Guid employeeId, DateOnly today, string what)
        {
            var shift = await repository.GetShiftAsync(shiftId) ?? throw new KeyNotFoundException("Shift not found.");
            if (shift.EmployeeId != employeeId)
                throw new ArgumentException($"That shift is not {what}.");
            if (shift.Date <= today)
                throw new ArgumentException("Only shifts after today can be swapped.");
            if (shift.ScheduleWeek?.Status != ScheduleWeekStatus.Confirmed)
                throw new ArgumentException("Only shifts in a confirmed week can be swapped.");
            return shift;
        }

        private async Task<ScheduleRequest> RequireReviewableAsync(Guid id, Guid reviewerId)
        {
            var request = await repository.GetRequestAsync(id) ?? throw new KeyNotFoundException("Request not found.");
            EnsurePending(request);
            if (request.EmployeeId == reviewerId)
                throw new ArgumentException("You can't review your own request.");
            return request;
        }

        private static void EnsurePending(ScheduleRequest request)
        {
            if (request.Status != ScheduleRequestStatus.Pending)
                throw new ArgumentException($"Request {request.RequestNumber} is already {request.Status.ToString().ToLowerInvariant()}.");
        }

        private static void Review(ScheduleRequest request, ScheduleRequestStatus status, ReviewScheduleRequest review, Guid reviewerId)
        {
            request.Status = status;
            request.ReviewedById = reviewerId;
            request.ReviewedAt = DateTime.UtcNow;
            request.ReviewNote = string.IsNullOrWhiteSpace(review?.Note) ? null : review.Note.Trim();
            request.UpdatedAt = DateTime.UtcNow;
        }

        private static void EnsureEditable(ScheduleWeek week, bool canEditConfirmed, DateOnly today)
        {
            if (week.WeekStart.AddDays(6) < today)
                throw new ArgumentException("This week is over and can't be changed anymore.");
            if (week.Status == ScheduleWeekStatus.Confirmed && !canEditConfirmed)
                throw new ArgumentException("This week is confirmed, so only a manager can change it.");
        }

        private static DateOnly RequireMonday(DateOnly weekStart) =>
            weekStart.DayOfWeek == DayOfWeek.Monday ? weekStart : throw new ArgumentException("A week starts on a Monday.");

        private static void Apply(ShiftTemplate template, SaveShiftTemplateRequest request)
        {
            template.Name = request.Name.Trim();
            template.Days = request.Days.ToDomain();
            template.StartTime = request.StartTime;
            template.EndTime = request.EndTime;
            template.Role = request.Role.ToDomain();
            template.RequiredStaff = request.RequiredStaff;
        }

        private async Task<ScheduleWeekDto> ToWeekDtoAsync(DateOnly start, ScheduleWeek? week, bool canSeeDrafts)
        {
            if (week is null)
                return new ScheduleWeekDto { WeekStart = start };

            var dto = new ScheduleWeekDto
            {
                WeekStart = start,
                Exists = true,
                Status = week.Status.ToDto(),
                GeneratedAt = week.GeneratedAt,
                ConfirmedAt = week.ConfirmedAt,
                ConfirmedByName = week.ConfirmedBy?.FullName,
            };

            if (week.Status == ScheduleWeekStatus.Draft && !canSeeDrafts)
            {
                dto.ShiftsHidden = true;
                return dto;
            }

            dto.Shifts = week.Shifts
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ThenBy(s => s.Role)
                .ThenBy(s => s.Employee?.FullName)
                .Select(s => s.ToDto())
                .ToList();
            dto.OpenShifts = week.Shifts.Count(s => s.EmployeeId is null);

            if (canSeeDrafts)
            {
                var staff = await repository.GetStaffAsync();
                dto.Staff = staff
                    .Where(e => e.ContractHours > 0 || week.Shifts.Any(s => s.EmployeeId == e.Id))
                    .Select(e =>
                    {
                        var own = week.Shifts.Where(s => s.EmployeeId == e.Id).ToList();
                        return new StaffHoursDto
                        {
                            EmployeeId = e.EmployeeId,
                            FullName = e.FullName,
                            Role = e.Role.ToDto(),
                            ContractHours = e.ContractHours,
                            PlannedHours = own.Sum(s => ScheduleRules.Hours(s.StartTime, s.EndTime)),
                            Shifts = own.Count,
                        };
                    })
                    .OrderBy(s => s.Role)
                    .ThenBy(s => s.EmployeeId, StringComparer.Ordinal)
                    .ToList();
            }

            return dto;
        }
    }
}
