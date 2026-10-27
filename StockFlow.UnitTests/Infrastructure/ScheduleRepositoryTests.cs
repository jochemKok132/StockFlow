using StockFlow.Application.Services.Scheduling;
using StockFlow.Domain.Entities.Scheduling;
using StockFlow.Infrastructure.Repositories.Scheduling;
using StockFlow.Shared.DTOs.Scheduling;

namespace StockFlow.UnitTests.Infrastructure
{
    public class ScheduleRepositoryTests : RepositoryTestBase
    {
        private static readonly DateOnly Monday = new(2026, 11, 2);
        private readonly ScheduleRepository _sut;

        public ScheduleRepositoryTests()
        {
            _sut = new ScheduleRepository(Context);
        }

        private async Task<ScheduleWeek> AddWeekAsync(DateOnly start, ScheduleWeekStatus status, params ScheduledShift[] shifts)
        {
            var week = new ScheduleWeek { Id = Guid.NewGuid(), WeekStart = start, Status = status, GeneratedAt = DateTime.UtcNow };
            foreach (var shift in shifts) week.Shifts.Add(shift);
            await _sut.AddWeekAsync(week);
            return week;
        }

        private static ScheduledShift Shift(DateOnly date, Guid? employeeId, int start = 8) => new()
        {
            Id = Guid.NewGuid(),
            Date = date,
            StartTime = new TimeOnly(start, 0),
            EndTime = new TimeOnly(start + 6, 0),
            Role = EmployeeRole.Cashier,
            EmployeeId = employeeId,
        };

        [Fact]
        public async Task GetWeekAsync_ByMonday_ReturnsTheWeekWithShiftsAndEmployees()
        {
            var employee = await AddEmployeeAsync("000010");
            await AddWeekAsync(Monday, ScheduleWeekStatus.Draft, Shift(Monday, employee.Id), Shift(Monday.AddDays(1), null));
            Context.ChangeTracker.Clear();

            var result = await _sut.GetWeekAsync(Monday);

            Assert.NotNull(result);
            Assert.Equal(2, result.Shifts.Count);
            Assert.Equal("000010", result.Shifts.Single(s => s.EmployeeId is not null).Employee?.EmployeeId);
            Assert.Null(await _sut.GetWeekAsync(Monday.AddDays(7)));
        }

        [Fact]
        public async Task GetConfirmedShiftsAsync_OnlyConfirmedWeeksInRange()
        {
            var employee = await AddEmployeeAsync("000010");
            await AddWeekAsync(Monday, ScheduleWeekStatus.Confirmed, Shift(Monday, employee.Id), Shift(Monday.AddDays(5), employee.Id));
            await AddWeekAsync(Monday.AddDays(7), ScheduleWeekStatus.Draft, Shift(Monday.AddDays(7), employee.Id));

            var result = await _sut.GetConfirmedShiftsAsync(employee.Id, Monday.AddDays(1), Monday.AddDays(13));

            Assert.Equal(Monday.AddDays(5), Assert.Single(result).Date);
        }

        [Fact]
        public async Task GetApprovedDaysOffAsync_OnlyApprovedDaysOffInRange()
        {
            var employee = await AddEmployeeAsync("000010");
            ScheduleRequest DayOff(int number, DateOnly date, ScheduleRequestStatus status) => new()
            {
                Id = Guid.NewGuid(), RequestNumber = number, Type = ScheduleRequestType.DayOff, Status = status, EmployeeId = employee.Id, Date = date, Reason = "x",
            };
            await _sut.AddRequestAsync(DayOff(1001, Monday, ScheduleRequestStatus.Approved));
            await _sut.AddRequestAsync(DayOff(1002, Monday.AddDays(1), ScheduleRequestStatus.Pending));
            await _sut.AddRequestAsync(DayOff(1003, Monday.AddDays(20), ScheduleRequestStatus.Approved));

            var result = await _sut.GetApprovedDaysOffAsync(Monday, Monday.AddDays(6));

            Assert.Equal((employee.Id, Monday), Assert.Single(result));
            Assert.True(await _sut.HasPendingRequestAsync(employee.Id, null, Monday.AddDays(1)));
            Assert.False(await _sut.HasPendingRequestAsync(employee.Id, null, Monday));
            Assert.Equal(1004, await _sut.GetNextRequestNumberAsync());
        }

        [Fact]
        public async Task GetStaffAsync_LeavesOutAdminAndTester()
        {
            await AddEmployeeAsync("000001", EmployeeRole.Admin);
            await AddEmployeeAsync("000023", EmployeeRole.Tester);
            await AddEmployeeAsync("000010", EmployeeRole.Cashier);

            var result = await _sut.GetStaffAsync();

            Assert.Equal("000010", Assert.Single(result).EmployeeId);
            Assert.Null(await _sut.GetStaffMemberAsync("000001"));
        }

        [Fact]
        public async Task RemoveShifts_CancelsOpenRequestsForThoseShifts()
        {
            var employee = await AddEmployeeAsync("000010");
            var shift = Shift(Monday, employee.Id);
            var week = await AddWeekAsync(Monday, ScheduleWeekStatus.Confirmed, shift);
            await _sut.AddRequestAsync(new ScheduleRequest { Id = Guid.NewGuid(), RequestNumber = 1001, Type = ScheduleRequestType.Swap, EmployeeId = employee.Id, ShiftId = shift.Id, Reason = "x" });

            _sut.RemoveShifts(week.Shifts.ToList());
            await _sut.SaveChangesAsync();

            var request = Assert.Single(await _sut.GetRequestsAsync(null, employee.Id));
            Assert.Equal(ScheduleRequestStatus.Cancelled, request.Status);
            Assert.Null(request.ShiftId);
        }

        [Fact]
        public async Task ScheduleService_RegenerateAddShiftAndSaveStaff_WorkAgainstTheDatabase()
        {
            var employee = await AddEmployeeAsync("000010");
            var service = new ScheduleService(_sut);
            await service.CreateTemplateAsync(new SaveShiftTemplateRequest { Name = "Morning", Days = WeekDaysDto.Monday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(14, 0), Role = EmployeeRoleDto.Cashier });
            var today = Monday.AddDays(-5);

            await service.SaveStaffAsync("000010", new SaveStaffRequest { ContractHours = 20, Availability = [new AvailabilityDto { Day = DayOfWeek.Tuesday, AvailableFrom = new TimeOnly(8, 0), AvailableUntil = new TimeOnly(21, 0) }] });
            await service.SaveStaffAsync("000010", new SaveStaffRequest { ContractHours = 24 });
            await service.GenerateWeekAsync(Monday, today);
            var again = await service.GenerateWeekAsync(Monday, today);
            var added = await service.AddShiftAsync(new SaveShiftRequest { Date = Monday.AddDays(2), StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(13, 0), Role = EmployeeRoleDto.Cashier, EmployeeId = "000010" }, false, today);
            Context.ChangeTracker.Clear();

            Assert.Equal("000010", Assert.Single(again.Shifts).EmployeeId);
            Assert.Equal(2, added.Shifts.Count);
            Assert.Equal(2, (await _sut.GetWeekAsync(Monday))!.Shifts.Count);
            Assert.Empty((await _sut.GetStaffMemberAsync("000010"))!.Availability);
            Assert.Equal(24, (await _sut.GetStaffMemberAsync("000010"))!.ContractHours);
        }

        [Fact]
        public async Task ScheduleService_GenerateAndConfirm_WorksAgainstTheDatabase()
        {
            var employee = await AddEmployeeAsync("000010");
            employee.ContractHours = 24;
            await Context.SaveChangesAsync();
            var service = new ScheduleService(_sut);
            await service.CreateTemplateAsync(new SaveShiftTemplateRequest { Name = "Morning", Days = WeekDaysDto.Monday | WeekDaysDto.Tuesday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(14, 0), Role = EmployeeRoleDto.Cashier });
            var today = Monday.AddDays(-5);

            Assert.Equal(4, await service.GenerateUpcomingWeeksAsync(today));
            var confirmed = await service.ConfirmWeekAsync(Monday, employee.Id, today);
            var mine = await service.GetMyShiftsAsync(employee.Id, today);

            Assert.Equal(ScheduleWeekStatusDto.Confirmed, confirmed.Status);
            Assert.Equal(2, confirmed.Shifts.Count(s => s.EmployeeId == "000010"));
            Assert.Equal(2, mine.Count);
        }
    }
}
