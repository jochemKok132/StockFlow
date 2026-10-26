using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Entities.Ordering
{
    public class PurchaseOrder : IEntity, ICreateable, IUpdatable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int OrderNumber { get; set; }
        public Guid SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Open;
        public DateTime OrderDate { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public Guid? ReceivedById { get; set; }
        public Employee? ReceivedBy { get; set; }
        public ICollection<PurchaseOrderLine> Lines { get; set; } = [];
    }

    public class PurchaseOrderLine : IEntity
    {
        public Guid Id { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public int Amount { get; set; }
        public int ReceivedAmount { get; set; }
        public OrderLineSource Source { get; set; } = OrderLineSource.Manual;
        public Guid? CustomerOrderId { get; set; }
        public Guid? AddedById { get; set; }
    }
}
