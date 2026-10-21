using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.Stock
{
    public class Sales : IEntity, ICreateable, IUpdatable, ISoftDeletable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }

        public required string SaleName { get; set; }
        public required string Description { get; set; }
        public SaleType SaleType { get; set; } = SaleType.Percentage;
        public int PercentageOff { get; set; }
        public int BuyAmount { get; set; }
        public int FreeAmount { get; set; }
        public decimal BundlePrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<Product> Products { get; set; } = [];
        public IEnumerable<string> SalesTags { get; set; } = [];
    }
}
