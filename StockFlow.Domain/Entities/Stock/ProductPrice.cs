using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;

namespace StockFlow.Domain.Entities.Stock
{
    public class ProductPrice : IEntity, ICreateable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public Guid? CreatedById { get; set; }
        public Employee? CreatedBy { get; set; }
    }
}
