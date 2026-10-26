using StockFlow.Shared.DTOs.Stock.ProductBrand;
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
                Description = productBrand.Description,
                Website = productBrand.Website,
                Country = productBrand.Country,
                ContactEmail = productBrand.ContactEmail,
                CustomerServicePhone = productBrand.CustomerServicePhone,
                Logo = productBrand.Logo,
                ProductCount = productBrand.Products.Count(p => !p.SoftDeleted),
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
                Description = Clean(dto.Description),
                Website = Clean(dto.Website),
                Country = Clean(dto.Country),
                ContactEmail = Clean(dto.ContactEmail),
                CustomerServicePhone = Clean(dto.CustomerServicePhone),
                Logo = dto.Logo is { Length: > 0 } ? dto.Logo : null,
                Id = Guid.NewGuid(),
            };
        }
        public static void ToProductBrandEntity(this ProductBrand entity, UpdateProductBrandDto dto)
        {
            entity.BrandName = dto.BrandName;
            entity.Description = Clean(dto.Description);
            entity.Website = Clean(dto.Website);
            entity.Country = Clean(dto.Country);
            entity.ContactEmail = Clean(dto.ContactEmail);
            entity.CustomerServicePhone = Clean(dto.CustomerServicePhone);
            entity.Logo = dto.Logo is { Length: > 0 } ? dto.Logo : null;
        }

        private static string? Clean(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
