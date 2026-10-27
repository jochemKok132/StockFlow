using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;

namespace StockFlow.Domain.Entities.AuditLogs
{
    public class ReturnLogs : IEntity, ICreateable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ReceiptId { get; set; } = string.Empty;
        public Guid CashRegisterLogId { get; set; }
        public CashRegisterLogs? CashRegisterLog { get; set; }
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public Guid? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public decimal RefundAmount { get; set; }
        public int PointsDeducted { get; set; }
        public ICollection<ReturnLogLine> Lines { get; set; } = [];
    }

    public class ReturnLogLine : IEntity
    {
        public Guid Id { get; set; }
        public Guid ReturnLogId { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public int Amount { get; set; }
        public decimal PriceAtTimeOfSale { get; set; }
        public decimal RefundAmount { get; set; }
    }
}
