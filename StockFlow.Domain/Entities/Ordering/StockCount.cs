using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Entities.Ordering
{
    public class StockCount : IEntity, ICreateable, IUpdatable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CountNumber { get; set; }
        public StockCountType Type { get; set; } = StockCountType.Manual;
        public StockCountStatus Status { get; set; } = StockCountStatus.Open;
        public Guid ShelfId { get; set; }
        public Shelf? Shelf { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Note { get; set; }
        public Guid? CreatedById { get; set; }
        public Employee? CreatedBy { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Guid? CompletedById { get; set; }
        public Employee? CompletedBy { get; set; }
        public ICollection<StockCountLine> Lines { get; set; } = [];
    }

    public class StockCountLine : IEntity
    {
        public Guid Id { get; set; }
        public Guid StockCountId { get; set; }
        public StockCount? StockCount { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public int ExpectedStock { get; set; }
        public int? CountedStock { get; set; }
        public int ExpiredAmount { get; set; }
        public DateTime? NextExpiryDate { get; set; }
    }
}
