using StockFlow.Domain.Entities.IEntities;

namespace StockFlow.Domain.Entities.Stock
{
    public class Supplier : IEntity, ICreateable, IUpdatable, ISoftDeletable
    {
        public static readonly Guid DefaultSupplierId = new("5a9e0d22-a35b-4656-b420-000000000001");

        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }
        public required string SupplierName { get; set; }
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public DayOfWeek OrderDay { get; set; } = DayOfWeek.Monday;
        public int LeadTimeDays { get; set; } = 2;
        public ICollection<Product> Products { get; set; } = [];
    }
}
