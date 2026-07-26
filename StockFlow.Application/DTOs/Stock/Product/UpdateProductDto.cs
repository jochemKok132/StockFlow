using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Stock.Product
{
    public class UpdateProductDto
    {
        public Guid Id { get; set; }

        public required string ProductName { get; set; }
        public required string Description { get; set; }
        public int Barcode { get; set; }
        public int Stock { get; set; }
        public int Location { get; set; }
        public byte[]? ProductImage { get; set; }

        public Guid BrandId { get; set; }
        public Guid ShelfId { get; set; }
        public Guid SaleId { get; set; }
    }
}
