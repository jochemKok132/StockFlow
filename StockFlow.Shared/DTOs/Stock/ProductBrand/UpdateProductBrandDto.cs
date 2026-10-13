using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.ProductBrand
{
    public class UpdateProductBrandDto
    {
        public Guid Id { get; set; }
        public required string BrandName { get; set; }
    }
}
