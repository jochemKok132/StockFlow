namespace StockFlow.Shared.DTOs.Enums
{
    [Flags]
    public enum WeekDaysDto
    {
        None = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64,
    }

    public enum ScheduleWeekStatusDto
    {
        Draft,
        Confirmed,
    }

    public enum ScheduleRequestTypeDto
    {
        Swap,
        DayOff,
    }

    public enum ScheduleRequestStatusDto
    {
        Pending,
        Approved,
        Rejected,
        Cancelled,
    }
}
