namespace StockFlow.Shared.Common
{
    public static class ScheduleRules
    {
        public const int WeeksAhead = 4;
        public const int MaxContractHours = 60;
        public const int MaxRequiredStaff = 10;
        public const int MaxShiftHours = 12;

        public static DateOnly WeekStart(DateOnly date) =>
            date.AddDays(-(((int)date.DayOfWeek + 6) % 7));

        public static DateOnly WeekStart(DateTime date) => WeekStart(DateOnly.FromDateTime(date));

        public static IEnumerable<DateOnly> UpcomingWeeks(DateOnly today)
        {
            var current = WeekStart(today);
            for (int week = 1; week <= WeeksAhead; week++)
                yield return current.AddDays(7 * week);
        }

        public static decimal Hours(TimeOnly start, TimeOnly end) =>
            Math.Round((decimal)(end - start).TotalHours, 2);

        public static bool Covers(TimeOnly availableFrom, TimeOnly availableUntil, TimeOnly start, TimeOnly end) =>
            availableFrom <= start && availableUntil >= end;
    }
}
