using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories.Scheduling;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Scheduling;
using StockFlow.Domain.Enums;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Repositories.Scheduling
{
    public class ScheduleRepository(ApplicationDBContext context) : IScheduleRepository
    {
        public const int FirstRequestNumber = 1001;
        private const int MaxRequests = 300;
        private static readonly EmployeeRole[] UnscheduledRoles = [EmployeeRole.Admin, EmployeeRole.Tester, EmployeeRole.None];

        public Task<List<ShiftTemplate>> GetTemplatesAsync() => context.ShiftTemplates.ToListAsync();

        public Task<ShiftTemplate?> GetTemplateAsync(Guid id) => context.ShiftTemplates.FirstOrDefaultAsync(t => t.Id == id);

        public async Task AddTemplateAsync(ShiftTemplate template)
        {
            template.CreatedAt = template.UpdatedAt = DateTime.UtcNow;
            context.ShiftTemplates.Add(template);
            await context.SaveChangesAsync();
        }

        public async Task RemoveTemplateAsync(ShiftTemplate template)
        {
            context.ShiftTemplates.Remove(template);
            await context.SaveChangesAsync();
        }

        public Task<List<Employee>> GetStaffAsync() =>
            context.Employees
                .Include(e => e.Availability)
                .Where(e => !UnscheduledRoles.Contains(e.Role))
                .ToListAsync();

        public Task<Employee?> GetStaffMemberAsync(string employeeNumber) =>
            context.Employees
                .Include(e => e.Availability)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeNumber && !UnscheduledRoles.Contains(e.Role));

        public void ReplaceAvailability(Employee employee, List<EmployeeAvailability> availability)
        {
            context.EmployeeAvailability.RemoveRange(employee.Availability);
            employee.Availability.Clear();
            foreach (var slot in availability)
            {
                employee.Availability.Add(slot);
                context.EmployeeAvailability.Add(slot);
            }
        }

        public Task<ScheduleWeek?> GetWeekAsync(DateOnly weekStart) =>
            context.ScheduleWeeks
                .Include(w => w.ConfirmedBy)
                .Include(w => w.Shifts).ThenInclude(s => s.Employee)
                .AsSplitQuery()
                .FirstOrDefaultAsync(w => w.WeekStart == weekStart);

        public Task<List<ScheduleWeek>> GetWeeksAsync(DateOnly from, DateOnly until) =>
            context.ScheduleWeeks
                .AsNoTracking()
                .Include(w => w.Shifts)
                .Where(w => w.WeekStart >= from && w.WeekStart <= until)
                .ToListAsync();

        public async Task AddWeekAsync(ScheduleWeek week)
        {
            week.CreatedAt = week.UpdatedAt = DateTime.UtcNow;
            context.ScheduleWeeks.Add(week);
            await context.SaveChangesAsync();
        }

        public void AddShifts(IEnumerable<ScheduledShift> shifts) => context.ScheduledShifts.AddRange(shifts);

        public void RemoveShifts(IEnumerable<ScheduledShift> shifts)
        {
            var ids = shifts.Select(s => s.Id).ToList();
            foreach (var request in context.ScheduleRequests.Where(r => (r.ShiftId != null && ids.Contains(r.ShiftId.Value)) || (r.TargetShiftId != null && ids.Contains(r.TargetShiftId.Value))))
            {
                if (request.ShiftId is Guid shiftId && ids.Contains(shiftId)) request.ShiftId = null;
                if (request.TargetShiftId is Guid targetId && ids.Contains(targetId)) request.TargetShiftId = null;
                if (request.Status == ScheduleRequestStatus.Pending)
                {
                    request.Status = ScheduleRequestStatus.Cancelled;
                    request.ReviewNote = "The shift was removed from the schedule.";
                    request.UpdatedAt = DateTime.UtcNow;
                }
            }
            context.ScheduledShifts.RemoveRange(shifts);
        }

        public Task<ScheduledShift?> GetShiftAsync(Guid id) =>
            context.ScheduledShifts
                .Include(s => s.ScheduleWeek)
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.Id == id);

        public Task<List<ScheduledShift>> GetConfirmedShiftsAsync(Guid employeeId, DateOnly from, DateOnly until) =>
            context.ScheduledShifts
                .AsNoTracking()
                .Include(s => s.Employee)
                .Where(s => s.EmployeeId == employeeId && s.Date >= from && s.Date <= until && s.ScheduleWeek!.Status == ScheduleWeekStatus.Confirmed)
                .ToListAsync();

        public Task<List<ScheduleRequest>> GetRequestsAsync(ScheduleRequestStatus? status, Guid? employeeId) =>
            WithDetails()
                .AsNoTracking()
                .Where(r => status == null || r.Status == status)
                .Where(r => employeeId == null || r.EmployeeId == employeeId || r.TargetEmployeeId == employeeId)
                .OrderByDescending(r => r.RequestNumber)
                .Take(MaxRequests)
                .ToListAsync();

        public Task<ScheduleRequest?> GetRequestAsync(Guid id) =>
            WithDetails().FirstOrDefaultAsync(r => r.Id == id);

        public async Task<int> GetNextRequestNumberAsync()
        {
            int? highest = await context.ScheduleRequests.MaxAsync(r => (int?)r.RequestNumber);
            return highest is int number ? number + 1 : FirstRequestNumber;
        }

        public async Task AddRequestAsync(ScheduleRequest request)
        {
            request.CreatedAt = request.UpdatedAt = DateTime.UtcNow;
            context.ScheduleRequests.Add(request);
            await context.SaveChangesAsync();
        }

        public Task<bool> HasPendingRequestAsync(Guid employeeId, Guid? shiftId, DateOnly? date) =>
            context.ScheduleRequests.AnyAsync(r =>
                r.EmployeeId == employeeId &&
                r.Status == ScheduleRequestStatus.Pending &&
                ((shiftId != null && r.ShiftId == shiftId) || (date != null && r.Date == date)));

        public async Task<HashSet<(Guid EmployeeId, DateOnly Date)>> GetApprovedDaysOffAsync(DateOnly from, DateOnly until)
        {
            var days = await context.ScheduleRequests
                .AsNoTracking()
                .Where(r => r.Type == ScheduleRequestType.DayOff && r.Status == ScheduleRequestStatus.Approved && r.Date >= from && r.Date <= until)
                .Select(r => new { r.EmployeeId, r.Date })
                .ToListAsync();
            return days.Select(d => (d.EmployeeId, d.Date!.Value)).ToHashSet();
        }

        public Task SaveChangesAsync() => context.SaveChangesAsync();

        private IQueryable<ScheduleRequest> WithDetails() =>
            context.ScheduleRequests
                .Include(r => r.Employee)
                .Include(r => r.TargetEmployee)
                .Include(r => r.ReviewedBy)
                .Include(r => r.Shift).ThenInclude(s => s!.Employee)
                .Include(r => r.TargetShift).ThenInclude(s => s!.Employee)
                .AsSplitQuery();
    }
}
