using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock.Sales;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Product
{
    public class CreateProductDto
    {
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public string ProductName { get; set; } = string.Empty;
        [MaxLength(FieldLimits.Description, ErrorMessage = FieldLimits.TooLongMessage)]
        public string Description { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int Location { get; set; }
        public decimal Price { get; set; }
        public byte[]? ProductImage { get; set; }
        public IEnumerable<string> SalesTags { get; set; } = [];

        public Guid BrandId { get; set; }
        public Guid ShelfId { get; set; }
        public IEnumerable<SalesDto> Sales { get; set; } = [];
        public ProductStatusDto Status { get; set; } = ProductStatusDto.Active;
        public Guid SupplierId { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public bool Orderable { get; set; } = true;
        public bool AutoOrder { get; set; }
        public bool CanExpire { get; set; }

    }
}
