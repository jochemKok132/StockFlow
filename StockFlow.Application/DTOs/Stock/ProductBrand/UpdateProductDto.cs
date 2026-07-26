using StockFlow.Application.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Stock.ProductBrand
{
    public class UpdateProductBrandDto
    {
        public Guid Id { get; set; }
        public required string BrandName { get; set; }
        public IEnumerable<UpdateProductDto> Products { get; set; } = [];
    }
}
