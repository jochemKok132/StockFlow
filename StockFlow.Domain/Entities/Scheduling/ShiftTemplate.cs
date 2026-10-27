using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Entities.Scheduling
{
    public class ShiftTemplate : IEntity, ICreateable, IUpdatable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required string Name { get; set; }
        public WeekDays Days { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public EmployeeRole Role { get; set; }
        public int RequiredStaff { get; set; } = 1;
    }
}
