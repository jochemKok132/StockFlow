using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.ProductBrand
{
    public class ProductBrandDto
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
        public int ProductCount { get; set; }
        public IEnumerable<ProductDto> Products { get; set; } = [];
    }
}
