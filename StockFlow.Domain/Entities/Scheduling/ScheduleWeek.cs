using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Entities.Scheduling
{
    public class ScheduleWeek : IEntity, ICreateable, IUpdatable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateOnly WeekStart { get; set; }
        public ScheduleWeekStatus Status { get; set; } = ScheduleWeekStatus.Draft;
        public DateTime GeneratedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public Guid? ConfirmedById { get; set; }
        public Employee? ConfirmedBy { get; set; }
        public ICollection<ScheduledShift> Shifts { get; set; } = [];
    }

    public class ScheduledShift : IEntity
    {
        public Guid Id { get; set; }
        public Guid ScheduleWeekId { get; set; }
        public ScheduleWeek? ScheduleWeek { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public EmployeeRole Role { get; set; }
        public Guid? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
