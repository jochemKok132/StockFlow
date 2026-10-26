using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.Stock
{
    public class Product : IEntity, ICreateable, IUpdatable, ISoftDeletable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }

        public required string ProductName { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int Barcode { get; set; }
        public int Stock { get; set; }
        public int Location { get; set; }
        public byte[]? ProductImage { get; set; }
        public IEnumerable<string> SalesTags { get; set; } = [];

        public Guid BrandId { get; set; }
        public ProductBrand? Brand { get; set; }
        public Guid ShelfId { get; set; }
        public Shelf? Shelf { get; set; }
        public ICollection<Sales> Sales { get; set; } = [];
        public ProductStatus Status { get; set; } = ProductStatus.Active;
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public bool Orderable { get; set; } = true;
        public bool AutoOrder { get; set; }
        public bool CanExpire { get; set; }
        public bool Recalled { get; set; }
        public string? RecallReason { get; set; }
        public DateTime? RecalledAt { get; set; }
        public bool OrderBlocked { get; set; }
        public string? OrderBlockReason { get; set; }
        public DateTime? OrderBlockedUntil { get; set; }
        public Guid SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public ICollection<ProductPrice> Prices { get; set; } = [];
        public ICollection<StockBatch> Batches { get; set; } = [];
    }
}
