using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Services.Scheduling;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Scheduling;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class ScheduleSeeder
    {
        private const WeekDays MondayToSaturday = WeekDays.Monday | WeekDays.Tuesday | WeekDays.Wednesday | WeekDays.Thursday | WeekDays.Friday | WeekDays.Saturday;
        private const WeekDays MondayToFriday = WeekDays.Monday | WeekDays.Tuesday | WeekDays.Wednesday | WeekDays.Thursday | WeekDays.Friday;

        private static readonly Dictionary<string, int> ContractHours = new()
        {
            ["000003"] = 32, ["000004"] = 32, ["000005"] = 32,
            ["000006"] = 24, ["000007"] = 24, ["000008"] = 16, ["000009"] = 16,
            ["000010"] = 32, ["000011"] = 32, ["000012"] = 24,
            ["000013"] = 16, ["000014"] = 16, ["000015"] = 16, ["000016"] = 12, ["000017"] = 12,
            ["000018"] = 12, ["000019"] = 12, ["000020"] = 8, ["000021"] = 8, ["000022"] = 16,
        };

        public static void UseScheduleSeeder(this DbContext context)
        {
            if (context.Set<ShiftTemplate>().Any() || context.Set<ScheduleWeek>().Any())
                return;

            var staff = context.Set<Employee>().Include(e => e.Availability).ToList();
            if (ContractHours.Keys.Any(number => staff.All(e => e.EmployeeId != number)))
                return;
            Employee Find(string number) => staff.First(e => e.EmployeeId == number);

            foreach (var (number, hours) in ContractHours)
                if (staff.FirstOrDefault(e => e.EmployeeId == number) is { } employee)
                    employee.ContractHours = hours;

            void AddAvailability(Employee employee, params (DayOfWeek Day, int From, int Until)[] windows)
            {
                foreach (var (day, from, until) in windows)
                    context.Set<EmployeeAvailability>().Add(new EmployeeAvailability
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = employee.Id,
                        Day = day,
                        AvailableFrom = new TimeOnly(from, 0),
                        AvailableUntil = new TimeOnly(until, 0),
                    });
            }

            AddAvailability(Find("000015"), (DayOfWeek.Wednesday, 12, 21), (DayOfWeek.Saturday, 7, 21));
            AddAvailability(Find("000020"),
                (DayOfWeek.Monday, 7, 13), (DayOfWeek.Tuesday, 7, 13), (DayOfWeek.Wednesday, 7, 13), (DayOfWeek.Thursday, 7, 13), (DayOfWeek.Friday, 7, 13));
            AddAvailability(Find("000012"),
                (DayOfWeek.Monday, 8, 21), (DayOfWeek.Tuesday, 8, 21), (DayOfWeek.Wednesday, 8, 21), (DayOfWeek.Thursday, 8, 21), (DayOfWeek.Friday, 8, 21), (DayOfWeek.Saturday, 8, 21));

            var templates = new List<ShiftTemplate>
            {
                Template("Opening shift leader", MondayToSaturday, 7, 30, 14, 30, EmployeeRole.ShiftLeader),
                Template("Closing shift leader", MondayToSaturday, 14, 30, 21, 15, EmployeeRole.ShiftLeader),
                Template("Sunday shift leader", WeekDays.Sunday, 9, 30, 17, 15, EmployeeRole.ShiftLeader),
                Template("Service desk early", MondayToSaturday, 9, 0, 15, 0, EmployeeRole.ServiceDesk),
                Template("Service desk late", MondayToSaturday, 15, 0, 21, 0, EmployeeRole.ServiceDesk),
                Template("Register morning", MondayToSaturday, 8, 0, 14, 30, EmployeeRole.Cashier),
                Template("Register evening", MondayToSaturday, 14, 30, 21, 0, EmployeeRole.Cashier),
                Template("Register Sunday", WeekDays.Sunday, 10, 0, 17, 0, EmployeeRole.Cashier),
                Template("Stock filling", MondayToSaturday, 7, 0, 12, 0, EmployeeRole.FloorWorker, 2),
                Template("Floor afternoon", MondayToSaturday, 12, 0, 18, 0, EmployeeRole.FloorWorker),
                Template("Floor evening", MondayToFriday, 17, 0, 21, 0, EmployeeRole.FloorWorker),
                Template("Floor Saturday evening", WeekDays.Saturday, 17, 0, 21, 0, EmployeeRole.FloorWorker),
                Template("Floor Sunday", WeekDays.Sunday, 10, 0, 17, 0, EmployeeRole.FloorWorker),
            };
            context.Set<ShiftTemplate>().AddRange(templates);

            var today = DateOnly.FromDateTime(DateTime.Today);
            var currentWeek = ScheduleRules.WeekStart(today);
            var nextWeek = currentWeek.AddDays(7);
            var nowUtc = DateTime.UtcNow;

            var approvedDayOff = Request(1001, ScheduleRequestType.DayOff, ScheduleRequestStatus.Approved, Find("000014"), nowUtc.AddDays(-9),
                "Family wedding.", date: nextWeek.AddDays(4), reviewer: SeedIds.Manager, note: "Enjoy the wedding!");
            var rejectedDayOff = Request(1002, ScheduleRequestType.DayOff, ScheduleRequestStatus.Rejected, Find("000007"), nowUtc.AddDays(-8),
                "Concert in the evening.", date: currentWeek.AddDays(5), reviewer: SeedIds.Manager, note: "Two colleagues are already off that day.");
            var daysOff = new HashSet<(Guid, DateOnly)> { (approvedDayOff.EmployeeId, approvedDayOff.Date!.Value) };

            ScheduleWeek Week(DateOnly start, DateTime confirmedAt) => new()
            {
                Id = Guid.NewGuid(),
                WeekStart = start,
                Status = ScheduleWeekStatus.Confirmed,
                CreatedAt = confirmedAt.AddDays(-3),
                UpdatedAt = confirmedAt,
                GeneratedAt = confirmedAt.AddDays(-3),
                ConfirmedAt = confirmedAt,
                ConfirmedById = SeedIds.Manager,
                Shifts = ScheduleGenerator.Generate(start, templates, staff, daysOff),
            };

            var current = Week(currentWeek, nowUtc.AddDays(-17));
            var next = Week(nextWeek, nowUtc.AddDays(-10));
            context.Set<ScheduleWeek>().AddRange(current, next);

            var swapShift = next.Shifts
                .Where(s => s.EmployeeId == SeedIds.CashierOne && s.Date > today)
                .OrderBy(s => s.Date)
                .FirstOrDefault(s => !next.Shifts.Any(o => o.EmployeeId == SeedIds.CashierTwo && o.Date == s.Date));
            var requests = new List<ScheduleRequest> { approvedDayOff, rejectedDayOff };
            if (swapShift is not null)
                requests.Add(Request(1003, ScheduleRequestType.Swap, ScheduleRequestStatus.Pending, Find("000010"), nowUtc.AddDays(-1),
                    "Doctor's appointment in the morning.", shift: swapShift, target: SeedIds.CashierTwo));
            requests.Add(Request(1004, ScheduleRequestType.DayOff, ScheduleRequestStatus.Pending, Find("000013"), nowUtc.AddHours(-5),
                "Exam at school.", date: nextWeek.AddDays(9)));

            context.Set<ScheduleRequest>().AddRange(requests);
            context.SaveChanges();
        }

        private static ShiftTemplate Template(string name, WeekDays days, int startHour, int startMinute, int endHour, int endMinute, EmployeeRole role, int required = 1) => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Days = days,
            StartTime = new TimeOnly(startHour, startMinute),
            EndTime = new TimeOnly(endHour, endMinute),
            Role = role,
            RequiredStaff = required,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-30),
        };

        private static ScheduleRequest Request(int number, ScheduleRequestType type, ScheduleRequestStatus status, Employee employee, DateTime createdAt, string reason,
            DateOnly? date = null, ScheduledShift? shift = null, Guid? target = null, Guid? reviewer = null, string? note = null) => new()
        {
            Id = Guid.NewGuid(),
            RequestNumber = number,
            Type = type,
            Status = status,
            EmployeeId = employee.Id,
            Date = date,
            ShiftId = shift?.Id,
            TargetEmployeeId = target,
            Reason = reason,
            ReviewedById = reviewer,
            ReviewedAt = reviewer is null ? null : createdAt.AddDays(1),
            ReviewNote = note,
            CreatedAt = createdAt,
            UpdatedAt = reviewer is null ? createdAt : createdAt.AddDays(1),
        };
    }
}
