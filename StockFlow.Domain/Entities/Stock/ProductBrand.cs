using StockFlow.Domain.Entities.IEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.Stock
{
    public class ProductBrand : IEntity, ICreateable, IUpdatable, ISoftDeletable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }

        public required string BrandName { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? Country { get; set; }
        public string? ContactEmail { get; set; }
        public string? CustomerServicePhone { get; set; }
        public byte[]? Logo { get; set; }
        public ICollection<Product> Products { get; set; } = [];
    }
}
