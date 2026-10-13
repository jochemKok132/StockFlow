using StockFlow.Shared.DTOs.Stock.ProductBrand;
using StockFlow.Shared.DTOs.Stock.Sales;
using StockFlow.Shared.DTOs.Stock.Shelf;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Product
{
    public class ProductBulkViewDto
    {
        public Guid Id { get; set; }
        public required string ProductName { get; set; }
        public int Barcode { get; set; }
        public int Stock { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }

        public string BrandName { get; set; }
        public bool Sale { get; set; }
    }
}
