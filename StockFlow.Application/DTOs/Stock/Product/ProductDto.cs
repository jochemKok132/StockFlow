using StockFlow.Application.DTOs.Stock.ProductBrand;
using StockFlow.Application.DTOs.Stock.Sales;
using StockFlow.Application.DTOs.Stock.Shelf;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Stock.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }

        public required string ProductName { get; set; }
        public required string Description { get; set; }
        public int Barcode { get; set; }
        public int Stock { get; set; }
        public int Location { get; set; }
        public byte[]? ProductImage { get; set; }

        public Guid BrandId { get; set; }
        public ProductBrandDto? Brand { get; set; }
        public Guid ShelfId { get; set; }
        public ShelfDto? Shelf { get; set; }
        public Guid SaleId { get; set; }
        public SalesDto? Sale { get; set; }
    }
}
