using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Entities.Scheduling
{
    public class ScheduleRequest : IEntity, ICreateable, IUpdatable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int RequestNumber { get; set; }
        public ScheduleRequestType Type { get; set; }
        public ScheduleRequestStatus Status { get; set; } = ScheduleRequestStatus.Pending;
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public Guid? ShiftId { get; set; }
        public ScheduledShift? Shift { get; set; }
        public Guid? TargetEmployeeId { get; set; }
        public Employee? TargetEmployee { get; set; }
        public Guid? TargetShiftId { get; set; }
        public ScheduledShift? TargetShift { get; set; }
        public DateOnly? Date { get; set; }
        public required string Reason { get; set; }
        public Guid? ReviewedById { get; set; }
        public Employee? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewNote { get; set; }
    }
}
