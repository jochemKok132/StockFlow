using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;

namespace StockFlow.Domain.Entities.Scheduling
{
    public class EmployeeAvailability : IEntity
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeOnly AvailableFrom { get; set; }
        public TimeOnly AvailableUntil { get; set; }
    }
}
