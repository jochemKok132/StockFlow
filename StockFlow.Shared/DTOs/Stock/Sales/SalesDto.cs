using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Sales
{
    public class SalesDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }

        public required string SaleName { get; set; }
        public required string Description { get; set; }
        public SaleTypeDto SaleType { get; set; } = SaleTypeDto.Percentage;
        public int PercentageOff { get; set; }
        public int BuyAmount { get; set; }
        public int FreeAmount { get; set; }
        public decimal BundlePrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int ItemsSold { get; set; }
        public IEnumerable<string> SalesTags { get; set; } = [];
        public IEnumerable<ProductDto> Products { get; set; } = [];

    }
}
