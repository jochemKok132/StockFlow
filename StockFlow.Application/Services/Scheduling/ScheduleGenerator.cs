using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Scheduling;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;

namespace StockFlow.Application.Services.Scheduling
{
    public static class ScheduleGenerator
    {
        public static WeekDays ToWeekDay(DayOfWeek day) => (WeekDays)(1 << (((int)day + 6) % 7));

        public static bool IsAvailable(Employee employee, DateOnly date, TimeOnly start, TimeOnly end) =>
            employee.Availability.Count == 0 ||
            employee.Availability.Any(a => a.Day == date.DayOfWeek && ScheduleRules.Covers(a.AvailableFrom, a.AvailableUntil, start, end));

        public static List<ScheduledShift> Generate(
            DateOnly weekStart,
            IEnumerable<ShiftTemplate> templates,
            IEnumerable<Employee> staff,
            ISet<(Guid EmployeeId, DateOnly Date)> daysOff)
        {
            var employees = staff.Where(e => e.ContractHours > 0).ToList();
            var planned = employees.ToDictionary(e => e.Id, _ => 0m);
            var workingDays = new HashSet<(Guid, DateOnly)>();
            var shifts = new List<ScheduledShift>();
            var ordered = templates
                .Where(t => t.RequiredStaff > 0 && t.StartTime < t.EndTime)
                .OrderBy(t => t.StartTime)
                .ThenBy(t => t.Role)
                .ThenBy(t => t.Name)
                .ToList();

            for (int offset = 0; offset < 7; offset++)
            {
                var date = weekStart.AddDays(offset);
                var weekDay = ToWeekDay(date.DayOfWeek);

                foreach (var template in ordered.Where(t => t.Days.HasFlag(weekDay)))
                {
                    decimal hours = ScheduleRules.Hours(template.StartTime, template.EndTime);
                    for (int slot = 0; slot < template.RequiredStaff; slot++)
                    {
                        var employee = employees
                            .Where(e => e.Role == template.Role
                                && !workingDays.Contains((e.Id, date))
                                && !daysOff.Contains((e.Id, date))
                                && planned[e.Id] + hours <= e.ContractHours
                                && IsAvailable(e, date, template.StartTime, template.EndTime))
                            .OrderByDescending(e => e.ContractHours - planned[e.Id])
                            .ThenBy(e => shifts.Count(s => s.EmployeeId == e.Id))
                            .ThenBy(e => e.EmployeeId, StringComparer.Ordinal)
                            .FirstOrDefault();

                        if (employee is not null)
                        {
                            planned[employee.Id] += hours;
                            workingDays.Add((employee.Id, date));
                        }

                        shifts.Add(new ScheduledShift
                        {
                            Id = Guid.NewGuid(),
                            Date = date,
                            StartTime = template.StartTime,
                            EndTime = template.EndTime,
                            Role = template.Role,
                            EmployeeId = employee?.Id,
                        });
                    }
                }
            }

            return shifts;
        }
    }
}
