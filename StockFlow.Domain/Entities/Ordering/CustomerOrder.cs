using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Entities.Ordering
{
    public class CustomerOrder : IEntity, ICreateable, IUpdatable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int OrderNumber { get; set; }
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public CustomerOrderStatus Status { get; set; } = CustomerOrderStatus.Waiting;
        public CustomerOrderSource Source { get; set; } = CustomerOrderSource.Manual;
        public string? Note { get; set; }
        public Guid? CreatedById { get; set; }
        public Employee? CreatedBy { get; set; }
        public DateTime? ReadyAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? ReceiptId { get; set; }
        public ICollection<CustomerOrderLine> Lines { get; set; } = [];
    }

    public class CustomerOrderLine : IEntity
    {
        public Guid Id { get; set; }
        public Guid CustomerOrderId { get; set; }
        public CustomerOrder? CustomerOrder { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public int Amount { get; set; }
        public int ReservedAmount { get; set; }
    }
}
