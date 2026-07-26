using StockFlow.Application.DTOs.Stock.ProductBrand;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.Stock
{
    public static class ProductBrandMappings
    {
        public static ProductBrandDto ToProductBrandDto(this ProductBrand productBrand)
        {
            return new ProductBrandDto()
            {
                BrandName = productBrand.BrandName,
                CreatedAt = productBrand.CreatedAt,
                Id = productBrand.Id,
                Products = productBrand.Products.Select(p => p.ToProductDto()),
                SoftDeleted = productBrand.SoftDeleted,
                SoftDeletedAt = productBrand.SoftDeletedAt,
                UpdatedAt = productBrand.UpdatedAt,
            };
        }
        public static ProductBrand ToProductBrandEntity(this CreateProductBrandDto dto)
        {
            return new ProductBrand()
            {
                BrandName = dto.BrandName,
                Id = Guid.NewGuid(),
            };
        }
        public static void ToProductBrandEntity(this ProductBrand entity, UpdateProductBrandDto dto)
        {
            entity.BrandName = dto.BrandName;
        }
    }
}
