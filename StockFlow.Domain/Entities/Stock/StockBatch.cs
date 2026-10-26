using StockFlow.Domain.Entities.IEntities;

namespace StockFlow.Domain.Entities.Stock
{
    public class StockBatch : IEntity, ICreateable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public int Amount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool CountForced { get; set; }
    }
}
