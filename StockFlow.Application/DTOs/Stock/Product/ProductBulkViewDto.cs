using StockFlow.Application.DTOs.Stock.ProductBrand;
using StockFlow.Application.DTOs.Stock.Sales;
using StockFlow.Application.DTOs.Stock.Shelf;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Stock.Product
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
