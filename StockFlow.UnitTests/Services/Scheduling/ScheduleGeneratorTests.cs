using StockFlow.Application.Services.Scheduling;
using StockFlow.Domain.Entities.Scheduling;

namespace StockFlow.UnitTests.Services.Scheduling
{
    public class ScheduleGeneratorTests
    {
        private static readonly DateOnly Monday = new(2026, 11, 2);

        private static Employee Staff(string number, EmployeeRole role, int hours, params (DayOfWeek Day, int From, int Until)[] availability)
        {
            var employee = new Employee { Id = Guid.NewGuid(), EmployeeId = number, FullName = $"Employee {number}", Role = role, ContractHours = hours };
            foreach (var (day, from, until) in availability)
                employee.Availability.Add(new EmployeeAvailability { Id = Guid.NewGuid(), EmployeeId = employee.Id, Day = day, AvailableFrom = new TimeOnly(from, 0), AvailableUntil = new TimeOnly(until, 0) });
            return employee;
        }

        private static ShiftTemplate Template(WeekDays days, int start, int end, EmployeeRole role = EmployeeRole.Cashier, int required = 1) => new()
        {
            Id = Guid.NewGuid(),
            Name = "Shift",
            Days = days,
            StartTime = new TimeOnly(start, 0),
            EndTime = new TimeOnly(end, 0),
            Role = role,
            RequiredStaff = required,
        };

        [Theory]
        [InlineData(DayOfWeek.Monday, WeekDays.Monday)]
        [InlineData(DayOfWeek.Saturday, WeekDays.Saturday)]
        [InlineData(DayOfWeek.Sunday, WeekDays.Sunday)]
        public void ToWeekDay_Day_ReturnsMatchingFlag(DayOfWeek day, WeekDays expected)
        {
            Assert.Equal(expected, ScheduleGenerator.ToWeekDay(day));
        }

        [Fact]
        public void Generate_TemplateOnTwoDays_CreatesAShiftPerDayAndRequiredEmployee()
        {
            var result = ScheduleGenerator.Generate(Monday, [Template(WeekDays.Monday | WeekDays.Sunday, 8, 14, required: 2)], [], new HashSet<(Guid, DateOnly)>());

            Assert.Equal(4, result.Count);
            Assert.Equal(2, result.Count(s => s.Date == Monday));
            Assert.Equal(2, result.Count(s => s.Date == Monday.AddDays(6)));
            Assert.All(result, s => Assert.Null(s.EmployeeId));
        }

        [Fact]
        public void Generate_OnlyAssignsEmployeesWithTheRole()
        {
            var floor = Staff("000013", EmployeeRole.FloorWorker, 40);
            var cashier = Staff("000010", EmployeeRole.Cashier, 40);

            var result = ScheduleGenerator.Generate(Monday, [Template(WeekDays.Monday, 8, 14)], [floor, cashier], new HashSet<(Guid, DateOnly)>());

            Assert.Equal(cashier.Id, Assert.Single(result).EmployeeId);
        }

        [Fact]
        public void Generate_NeverMoreThanContractHours()
        {
            var cashier = Staff("000010", EmployeeRole.Cashier, 12);
            var days = WeekDays.Monday | WeekDays.Tuesday | WeekDays.Wednesday;

            var result = ScheduleGenerator.Generate(Monday, [Template(days, 8, 14)], [cashier], new HashSet<(Guid, DateOnly)>());

            Assert.Equal(2, result.Count(s => s.EmployeeId == cashier.Id));
            Assert.Single(result, s => s.EmployeeId is null);
        }

        [Fact]
        public void Generate_OneShiftPerEmployeePerDay()
        {
            var cashier = Staff("000010", EmployeeRole.Cashier, 40);

            var result = ScheduleGenerator.Generate(Monday, [Template(WeekDays.Monday, 8, 12), Template(WeekDays.Monday, 13, 17)], [cashier], new HashSet<(Guid, DateOnly)>());

            Assert.Single(result, s => s.EmployeeId == cashier.Id);
        }

        [Fact]
        public void Generate_RespectsAvailability()
        {
            var student = Staff("000010", EmployeeRole.Cashier, 40, (DayOfWeek.Tuesday, 12, 21));

            var result = ScheduleGenerator.Generate(Monday, [Template(WeekDays.Monday | WeekDays.Tuesday, 14, 20), Template(WeekDays.Tuesday, 8, 13)], [student], new HashSet<(Guid, DateOnly)>());

            var assigned = Assert.Single(result, s => s.EmployeeId == student.Id);
            Assert.Equal(Monday.AddDays(1), assigned.Date);
            Assert.Equal(new TimeOnly(14, 0), assigned.StartTime);
        }

        [Fact]
        public void Generate_SkipsApprovedDaysOff()
        {
            var cashier = Staff("000010", EmployeeRole.Cashier, 40);

            var result = ScheduleGenerator.Generate(Monday, [Template(WeekDays.Monday, 8, 14)], [cashier], new HashSet<(Guid, DateOnly)> { (cashier.Id, Monday) });

            Assert.Null(Assert.Single(result).EmployeeId);
        }

        [Fact]
        public void Generate_GivesTheShiftToWhoHasTheMostHoursLeft()
        {
            var small = Staff("000010", EmployeeRole.Cashier, 8);
            var big = Staff("000011", EmployeeRole.Cashier, 32);

            var result = ScheduleGenerator.Generate(Monday, [Template(WeekDays.Monday | WeekDays.Tuesday | WeekDays.Wednesday | WeekDays.Thursday | WeekDays.Friday, 9, 15)], [small, big], new HashSet<(Guid, DateOnly)>());

            Assert.Equal(1, result.Count(s => s.EmployeeId == small.Id));
            Assert.Equal(4, result.Count(s => s.EmployeeId == big.Id));
        }

        [Fact]
        public void Generate_ZeroContractHours_IsNeverScheduled()
        {
            var manager = Staff("000002", EmployeeRole.Cashier, 0);

            var result = ScheduleGenerator.Generate(Monday, [Template(WeekDays.Monday, 8, 14)], [manager], new HashSet<(Guid, DateOnly)>());

            Assert.Null(Assert.Single(result).EmployeeId);
        }
    }
}
