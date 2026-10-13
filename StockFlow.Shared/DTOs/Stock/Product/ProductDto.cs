using StockFlow.Shared.DTOs.Stock.ProductBrand;
using StockFlow.Shared.DTOs.Stock.Sales;
using StockFlow.Shared.DTOs.Stock.Shelf;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Barcode { get; set; }
        public int Stock { get; set; }
        public int Location { get; set; }
        public byte[]? ProductImage { get; set; }
        public IEnumerable<string> SalesTags { get; set; } = [];

        public Guid BrandId { get; set; }
        public ProductBrandDto? Brand { get; set; }
        public Guid ShelfId { get; set; }
        public ShelfDto? Shelf { get; set; }
        public IEnumerable<SalesDto> Sales { get; set; } = [];

    }
}
