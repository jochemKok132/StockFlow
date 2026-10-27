using StockFlow.Application.Interfaces.Repositories.Scheduling;
using StockFlow.Application.Services.Scheduling;
using StockFlow.Domain.Entities.Scheduling;
using StockFlow.Shared.DTOs.Scheduling;

namespace StockFlow.UnitTests.Services.Scheduling
{
    public class ScheduleServiceTests
    {
        private static readonly DateOnly Today = new(2026, 10, 28);
        private static readonly DateOnly NextMonday = new(2026, 11, 2);

        private readonly Mock<IScheduleRepository> _repository = new();
        private readonly ScheduleService _sut;
        private readonly Employee _cashierOne = Staff("000010", EmployeeRole.Cashier);
        private readonly Employee _cashierTwo = Staff("000011", EmployeeRole.Cashier);
        private readonly Employee _floorWorker = Staff("000013", EmployeeRole.FloorWorker);
        private readonly Guid _managerId = Guid.NewGuid();

        public ScheduleServiceTests()
        {
            _sut = new ScheduleService(_repository.Object);
            _repository.Setup(r => r.GetStaffAsync()).ReturnsAsync(() => [_cashierOne, _cashierTwo, _floorWorker]);
            _repository.Setup(r => r.GetStaffMemberAsync(It.IsAny<string>())).ReturnsAsync((string number) => new[] { _cashierOne, _cashierTwo, _floorWorker }.FirstOrDefault(e => e.EmployeeId == number));
            _repository.Setup(r => r.GetApprovedDaysOffAsync(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).ReturnsAsync([]);
            _repository.Setup(r => r.GetTemplatesAsync()).ReturnsAsync([]);
        }

        private static Employee Staff(string number, EmployeeRole role) =>
            new() { Id = Guid.NewGuid(), EmployeeId = number, FullName = $"Employee {number}", Role = role, ContractHours = 24 };

        private ScheduleWeek Week(DateOnly start, ScheduleWeekStatus status, params ScheduledShift[] shifts)
        {
            var week = new ScheduleWeek { Id = Guid.NewGuid(), WeekStart = start, Status = status };
            foreach (var shift in shifts)
            {
                shift.ScheduleWeekId = week.Id;
                shift.ScheduleWeek = week;
                week.Shifts.Add(shift);
            }
            _repository.Setup(r => r.GetWeekAsync(start)).ReturnsAsync(week);
            foreach (var shift in shifts)
                _repository.Setup(r => r.GetShiftAsync(shift.Id)).ReturnsAsync(shift);
            return week;
        }

        private static ScheduledShift Shift(DateOnly date, Employee? employee, EmployeeRole role = EmployeeRole.Cashier) => new()
        {
            Id = Guid.NewGuid(),
            Date = date,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(14, 0),
            Role = role,
            EmployeeId = employee?.Id,
            Employee = employee,
        };

        private ScheduleRequest StoredRequest(ScheduleRequest request)
        {
            _repository.Setup(r => r.GetRequestAsync(request.Id)).ReturnsAsync(request);
            return request;
        }

        private static SaveShiftRequest ShiftRequest(DateOnly date, string? employeeId, EmployeeRoleDto role = EmployeeRoleDto.Cashier) => new()
        {
            Date = date,
            StartTime = new TimeOnly(8, 0),
            EndTime = new TimeOnly(14, 0),
            Role = role,
            EmployeeId = employeeId,
        };

        [Fact]
        public async Task GenerateWeekAsync_CurrentWeek_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GenerateWeekAsync(new DateOnly(2026, 10, 26), Today));
        }

        [Fact]
        public async Task GenerateWeekAsync_NotAMonday_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GenerateWeekAsync(NextMonday.AddDays(1), Today));
        }

        [Fact]
        public async Task GenerateWeekAsync_ConfirmedWeek_ThrowsArgumentException()
        {
            Week(NextMonday, ScheduleWeekStatus.Confirmed);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GenerateWeekAsync(NextMonday, Today));
        }

        [Fact]
        public async Task GenerateWeekAsync_DraftWeek_ReplacesItsShifts()
        {
            var old = Shift(NextMonday, _cashierOne);
            var week = Week(NextMonday, ScheduleWeekStatus.Draft, old);
            _repository.Setup(r => r.GetTemplatesAsync()).ReturnsAsync([new ShiftTemplate { Id = Guid.NewGuid(), Name = "Morning", Days = WeekDays.Tuesday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(14, 0), Role = EmployeeRole.Cashier, RequiredStaff = 1 }]);

            await _sut.GenerateWeekAsync(NextMonday, Today);

            _repository.Verify(r => r.RemoveShifts(It.Is<IEnumerable<ScheduledShift>>(s => s.Single() == old)), Times.Once);
            Assert.Equal(NextMonday.AddDays(1), Assert.Single(week.Shifts).Date);
        }

        [Fact]
        public async Task GenerateUpcomingWeeksAsync_CreatesOnlyMissingWeeks()
        {
            _repository.Setup(r => r.GetTemplatesAsync()).ReturnsAsync([new ShiftTemplate { Id = Guid.NewGuid(), Name = "Morning", Days = WeekDays.Monday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(14, 0), Role = EmployeeRole.Cashier }]);
            Week(NextMonday, ScheduleWeekStatus.Confirmed);

            int created = await _sut.GenerateUpcomingWeeksAsync(Today);

            Assert.Equal(3, created);
            _repository.Verify(r => r.AddWeekAsync(It.Is<ScheduleWeek>(w => w.Status == ScheduleWeekStatus.Draft && w.Shifts.Count == 1)), Times.Exactly(3));
        }

        [Fact]
        public async Task GenerateUpcomingWeeksAsync_NoShiftsSetUp_CreatesNothing()
        {
            Assert.Equal(0, await _sut.GenerateUpcomingWeeksAsync(Today));
            _repository.Verify(r => r.AddWeekAsync(It.IsAny<ScheduleWeek>()), Times.Never);
        }

        [Fact]
        public async Task ConfirmWeekAsync_DraftWeek_ConfirmsIt()
        {
            var week = Week(NextMonday, ScheduleWeekStatus.Draft);

            await _sut.ConfirmWeekAsync(NextMonday, _managerId, Today);

            Assert.Equal(ScheduleWeekStatus.Confirmed, week.Status);
            Assert.Equal(_managerId, week.ConfirmedById);
            _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ConfirmWeekAsync_AlreadyConfirmed_ThrowsArgumentException()
        {
            Week(NextMonday, ScheduleWeekStatus.Confirmed);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ConfirmWeekAsync(NextMonday, _managerId, Today));
        }

        [Fact]
        public async Task GetWeekAsync_DraftForAnEmployee_HidesTheShifts()
        {
            Week(NextMonday, ScheduleWeekStatus.Draft, Shift(NextMonday, _cashierOne));

            var result = await _sut.GetWeekAsync(NextMonday.AddDays(3), canSeeDrafts: false);

            Assert.True(result.ShiftsHidden);
            Assert.Empty(result.Shifts);
            Assert.Empty(result.Staff);
        }

        [Fact]
        public async Task GetWeekAsync_DraftForAnEditor_ShowsShiftsAndPlannedHours()
        {
            Week(NextMonday, ScheduleWeekStatus.Draft, Shift(NextMonday, _cashierOne), Shift(NextMonday, null));

            var result = await _sut.GetWeekAsync(NextMonday, canSeeDrafts: true);

            Assert.Equal(2, result.Shifts.Count);
            Assert.Equal(1, result.OpenShifts);
            Assert.Equal(6m, result.Staff.Single(s => s.EmployeeId == "000010").PlannedHours);
        }

        [Fact]
        public async Task AddShiftAsync_WrongRole_ThrowsArgumentException()
        {
            Week(NextMonday, ScheduleWeekStatus.Draft);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddShiftAsync(ShiftRequest(NextMonday, "000013"), false, Today));
        }

        [Fact]
        public async Task AddShiftAsync_EmployeeAlreadyWorksThatDay_ThrowsArgumentException()
        {
            Week(NextMonday, ScheduleWeekStatus.Draft, Shift(NextMonday, _cashierOne));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddShiftAsync(ShiftRequest(NextMonday, "000010"), false, Today));
        }

        [Fact]
        public async Task AddShiftAsync_ConfirmedWeekWithoutManagerRights_ThrowsArgumentException()
        {
            Week(NextMonday, ScheduleWeekStatus.Confirmed);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AddShiftAsync(ShiftRequest(NextMonday, null), false, Today));
        }

        [Fact]
        public async Task AddShiftAsync_ValidShift_AddsItToTheWeek()
        {
            var week = Week(NextMonday, ScheduleWeekStatus.Confirmed);

            await _sut.AddShiftAsync(ShiftRequest(NextMonday, "000010"), true, Today);

            Assert.Equal(_cashierOne.Id, Assert.Single(week.Shifts).EmployeeId);
            _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateShiftAsync_EndBeforeStart_ThrowsArgumentException()
        {
            var shift = Shift(NextMonday, null);
            Week(NextMonday, ScheduleWeekStatus.Draft, shift);
            var request = ShiftRequest(NextMonday, null);
            request.EndTime = new TimeOnly(7, 0);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateShiftAsync(shift.Id, request, false, Today));
        }

        [Fact]
        public async Task DeleteShiftAsync_PastWeek_ThrowsArgumentException()
        {
            var lastWeek = new DateOnly(2026, 10, 19);
            var shift = Shift(lastWeek, _cashierOne);
            Week(lastWeek, ScheduleWeekStatus.Confirmed, shift);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.DeleteShiftAsync(shift.Id, true, Today));
        }

        [Fact]
        public async Task CreateRequestAsync_SwapOfSomeoneElsesShift_ThrowsArgumentException()
        {
            var shift = Shift(NextMonday, _cashierTwo);
            Week(NextMonday, ScheduleWeekStatus.Confirmed, shift);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateRequestAsync(
                new CreateScheduleRequest { Type = ScheduleRequestTypeDto.Swap, ShiftId = shift.Id, TargetEmployeeId = "000011", Reason = "Dentist" }, _cashierOne.Id, Today));
        }

        [Fact]
        public async Task CreateRequestAsync_SwapWithColleagueOfOtherRole_ThrowsArgumentException()
        {
            var shift = Shift(NextMonday, _cashierOne);
            Week(NextMonday, ScheduleWeekStatus.Confirmed, shift);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateRequestAsync(
                new CreateScheduleRequest { Type = ScheduleRequestTypeDto.Swap, ShiftId = shift.Id, TargetEmployeeId = "000013", Reason = "Dentist" }, _cashierOne.Id, Today));
        }

        [Fact]
        public async Task CreateRequestAsync_SwapInDraftWeek_ThrowsArgumentException()
        {
            var shift = Shift(NextMonday, _cashierOne);
            Week(NextMonday, ScheduleWeekStatus.Draft, shift);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateRequestAsync(
                new CreateScheduleRequest { Type = ScheduleRequestTypeDto.Swap, ShiftId = shift.Id, TargetEmployeeId = "000011", Reason = "Dentist" }, _cashierOne.Id, Today));
        }

        [Fact]
        public async Task CreateRequestAsync_ValidSwap_SavesAPendingRequestWithTheNextNumber()
        {
            var shift = Shift(NextMonday, _cashierOne);
            Week(NextMonday, ScheduleWeekStatus.Confirmed, shift);
            ScheduleRequest? saved = null;
            _repository.Setup(r => r.GetNextRequestNumberAsync()).ReturnsAsync(1005);
            _repository.Setup(r => r.AddRequestAsync(It.IsAny<ScheduleRequest>())).Callback<ScheduleRequest>(r => { saved = r; StoredRequest(r); });

            var result = await _sut.CreateRequestAsync(
                new CreateScheduleRequest { Type = ScheduleRequestTypeDto.Swap, ShiftId = shift.Id, TargetEmployeeId = "000011", Reason = " Dentist " }, _cashierOne.Id, Today);

            Assert.Equal(1005, result.RequestNumber);
            Assert.Equal(ScheduleRequestStatus.Pending, saved?.Status);
            Assert.Equal(_cashierTwo.Id, saved?.TargetEmployeeId);
            Assert.Equal("Dentist", saved?.Reason);
        }

        [Fact]
        public async Task CreateRequestAsync_DayOffToday_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateRequestAsync(
                new CreateScheduleRequest { Type = ScheduleRequestTypeDto.DayOff, Date = Today, Reason = "Sick" }, _cashierOne.Id, Today));
        }

        [Fact]
        public async Task CreateRequestAsync_DayOffAlreadyAsked_ThrowsArgumentException()
        {
            _repository.Setup(r => r.HasPendingRequestAsync(_cashierOne.Id, null, NextMonday)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateRequestAsync(
                new CreateScheduleRequest { Type = ScheduleRequestTypeDto.DayOff, Date = NextMonday, Reason = "Wedding" }, _cashierOne.Id, Today));
        }

        [Fact]
        public async Task ApproveRequestAsync_OwnRequest_ThrowsArgumentException()
        {
            var request = StoredRequest(new ScheduleRequest { Id = Guid.NewGuid(), Type = ScheduleRequestType.DayOff, EmployeeId = _managerId, Date = NextMonday, Reason = "x" });

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ApproveRequestAsync(request.Id, new ReviewScheduleRequest(), _managerId, Today));
        }

        [Fact]
        public async Task ApproveRequestAsync_AlreadyRejected_ThrowsArgumentException()
        {
            var request = StoredRequest(new ScheduleRequest { Id = Guid.NewGuid(), Type = ScheduleRequestType.DayOff, Status = ScheduleRequestStatus.Rejected, EmployeeId = _cashierOne.Id, Date = NextMonday, Reason = "x" });

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ApproveRequestAsync(request.Id, new ReviewScheduleRequest(), _managerId, Today));
        }

        [Fact]
        public async Task ApproveRequestAsync_Swap_GivesTheShiftToTheColleague()
        {
            var shift = Shift(NextMonday, _cashierOne);
            Week(NextMonday, ScheduleWeekStatus.Confirmed, shift);
            var request = StoredRequest(new ScheduleRequest { Id = Guid.NewGuid(), Type = ScheduleRequestType.Swap, EmployeeId = _cashierOne.Id, Employee = _cashierOne, ShiftId = shift.Id, Shift = shift, TargetEmployeeId = _cashierTwo.Id, TargetEmployee = _cashierTwo, Reason = "x" });

            await _sut.ApproveRequestAsync(request.Id, new ReviewScheduleRequest { Note = " ok " }, _managerId, Today);

            Assert.Equal(_cashierTwo.Id, shift.EmployeeId);
            Assert.Equal(ScheduleRequestStatus.Approved, request.Status);
            Assert.Equal(_managerId, request.ReviewedById);
            Assert.Equal("ok", request.ReviewNote);
        }

        [Fact]
        public async Task ApproveRequestAsync_Exchange_SwapsBothShifts()
        {
            var mine = Shift(NextMonday, _cashierOne);
            var theirs = Shift(NextMonday.AddDays(2), _cashierTwo);
            Week(NextMonday, ScheduleWeekStatus.Confirmed, mine, theirs);
            var request = StoredRequest(new ScheduleRequest { Id = Guid.NewGuid(), Type = ScheduleRequestType.Swap, EmployeeId = _cashierOne.Id, ShiftId = mine.Id, Shift = mine, TargetEmployeeId = _cashierTwo.Id, TargetShiftId = theirs.Id, TargetShift = theirs, Reason = "x" });

            await _sut.ApproveRequestAsync(request.Id, new ReviewScheduleRequest(), _managerId, Today);

            Assert.Equal(_cashierTwo.Id, mine.EmployeeId);
            Assert.Equal(_cashierOne.Id, theirs.EmployeeId);
        }

        [Fact]
        public async Task ApproveRequestAsync_ColleagueAlreadyWorksThatDay_ThrowsArgumentException()
        {
            var shift = Shift(NextMonday, _cashierOne);
            Week(NextMonday, ScheduleWeekStatus.Confirmed, shift, Shift(NextMonday, _cashierTwo));
            var request = StoredRequest(new ScheduleRequest { Id = Guid.NewGuid(), Type = ScheduleRequestType.Swap, EmployeeId = _cashierOne.Id, ShiftId = shift.Id, Shift = shift, TargetEmployeeId = _cashierTwo.Id, Reason = "x" });

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ApproveRequestAsync(request.Id, new ReviewScheduleRequest(), _managerId, Today));
            Assert.Equal(_cashierOne.Id, shift.EmployeeId);
        }

        [Fact]
        public async Task ApproveRequestAsync_DayOff_OpensTheShiftsOfThatDay()
        {
            var shift = Shift(NextMonday, _cashierOne);
            var otherDay = Shift(NextMonday.AddDays(1), _cashierOne);
            Week(NextMonday, ScheduleWeekStatus.Confirmed, shift, otherDay);
            var request = StoredRequest(new ScheduleRequest { Id = Guid.NewGuid(), Type = ScheduleRequestType.DayOff, EmployeeId = _cashierOne.Id, Date = NextMonday, Reason = "x" });

            await _sut.ApproveRequestAsync(request.Id, new ReviewScheduleRequest(), _managerId, Today);

            Assert.Null(shift.EmployeeId);
            Assert.Equal(_cashierOne.Id, otherDay.EmployeeId);
        }

        [Fact]
        public async Task RejectRequestAsync_Pending_SetsRejected()
        {
            var request = StoredRequest(new ScheduleRequest { Id = Guid.NewGuid(), Type = ScheduleRequestType.DayOff, EmployeeId = _cashierOne.Id, Date = NextMonday, Reason = "x" });

            await _sut.RejectRequestAsync(request.Id, new ReviewScheduleRequest { Note = "Too busy" }, _managerId);

            Assert.Equal(ScheduleRequestStatus.Rejected, request.Status);
            Assert.Equal("Too busy", request.ReviewNote);
        }

        [Fact]
        public async Task CancelRequestAsync_SomeoneElsesRequest_ThrowsArgumentException()
        {
            var request = StoredRequest(new ScheduleRequest { Id = Guid.NewGuid(), Type = ScheduleRequestType.DayOff, EmployeeId = _cashierOne.Id, Date = NextMonday, Reason = "x" });

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CancelRequestAsync(request.Id, _cashierTwo.Id));
        }

        [Fact]
        public async Task GetColleaguesAsync_ReturnsColleaguesWithTheSameRoleAndTheirShifts()
        {
            var shift = Shift(NextMonday, _cashierTwo);
            _repository.Setup(r => r.GetConfirmedShiftsAsync(_cashierTwo.Id, Today.AddDays(1), It.IsAny<DateOnly>())).ReturnsAsync([shift]);

            var result = await _sut.GetColleaguesAsync(_cashierOne.Id, Today);

            var colleague = Assert.Single(result);
            Assert.Equal("000011", colleague.EmployeeId);
            Assert.Equal(shift.Id, Assert.Single(colleague.UpcomingShifts).Id);
        }

        [Fact]
        public async Task SaveStaffAsync_TwoWindowsOnOneDay_ThrowsArgumentException()
        {
            var request = new SaveStaffRequest
            {
                ContractHours = 16,
                Availability =
                [
                    new AvailabilityDto { Day = DayOfWeek.Monday, AvailableFrom = new TimeOnly(8, 0), AvailableUntil = new TimeOnly(12, 0) },
                    new AvailabilityDto { Day = DayOfWeek.Monday, AvailableFrom = new TimeOnly(13, 0), AvailableUntil = new TimeOnly(17, 0) },
                ],
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.SaveStaffAsync("000010", request));
        }

        [Fact]
        public async Task SaveStaffAsync_Valid_SavesHoursAndAvailability()
        {
            var request = new SaveStaffRequest { ContractHours = 16, Availability = [new AvailabilityDto { Day = DayOfWeek.Saturday, AvailableFrom = new TimeOnly(8, 0), AvailableUntil = new TimeOnly(17, 0) }] };

            var result = await _sut.SaveStaffAsync("000010", request);

            Assert.Equal(16, _cashierOne.ContractHours);
            Assert.Equal(16, result.ContractHours);
            _repository.Verify(r => r.ReplaceAvailability(_cashierOne, It.Is<List<EmployeeAvailability>>(a => a.Single().Day == DayOfWeek.Saturday)), Times.Once);
        }

        [Theory]
        [InlineData(WeekDaysDto.None, 8, 14, 1)]
        [InlineData(WeekDaysDto.Monday, 14, 8, 1)]
        [InlineData(WeekDaysDto.Monday, 6, 20, 1)]
        [InlineData(WeekDaysDto.Monday, 8, 14, 0)]
        [InlineData(WeekDaysDto.Monday, 8, 14, 11)]
        public async Task CreateTemplateAsync_InvalidShift_ThrowsArgumentException(WeekDaysDto days, int start, int end, int required)
        {
            var request = new SaveShiftTemplateRequest { Name = "Morning", Days = days, StartTime = new TimeOnly(start, 0), EndTime = new TimeOnly(end, 0), Role = EmployeeRoleDto.Cashier, RequiredStaff = required };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateTemplateAsync(request));
        }

        [Fact]
        public async Task CreateTemplateAsync_AdminRole_ThrowsArgumentException()
        {
            var request = new SaveShiftTemplateRequest { Name = "Morning", Days = WeekDaysDto.Monday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(14, 0), Role = EmployeeRoleDto.Admin };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateTemplateAsync(request));
        }

        [Fact]
        public async Task CreateTemplateAsync_Valid_AddsTemplate()
        {
            var request = new SaveShiftTemplateRequest { Name = " Morning ", Days = WeekDaysDto.Monday | WeekDaysDto.Friday, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(14, 30), Role = EmployeeRoleDto.Cashier, RequiredStaff = 2 };

            var result = await _sut.CreateTemplateAsync(request);

            Assert.Equal("Morning", result.Name);
            Assert.Equal(6.5m, result.Hours);
            _repository.Verify(r => r.AddTemplateAsync(It.Is<ShiftTemplate>(t => t.Days == (WeekDays.Monday | WeekDays.Friday) && t.RequiredStaff == 2)), Times.Once);
        }
    }
}
