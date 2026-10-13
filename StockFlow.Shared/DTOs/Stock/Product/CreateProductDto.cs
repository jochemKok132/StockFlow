using StockFlow.Shared.DTOs.Stock.Sales;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Product
{
    public class CreateProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int Location { get; set; }
        public decimal Price { get; set; }
        public byte[]? ProductImage { get; set; }
        public IEnumerable<string> SalesTags { get; set; } = [];

        public Guid BrandId { get; set; }
        public Guid ShelfId { get; set; }
        public IEnumerable<SalesDto> Sales { get; set; } = [];

    }
}
