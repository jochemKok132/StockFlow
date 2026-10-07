using StockFlow.Domain.Entities.CashRegister;
using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;

namespace StockFlow.Domain.Entities.AuditLogs
{
    public class CashRegisterLogs : IEntity, ICreateable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public Guid? CustomerId { get; set; }
        public Customer? Customer{ get; set; }
        public double TotalPrice { get; set; }
        public double TotalOff { get; set; }
        public ICollection<RegisterItemDetail> ProductsSold { get; set; } = [];
    }
}
