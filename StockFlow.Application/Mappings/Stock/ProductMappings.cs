using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.Stock
{
    public static class ProductMappings
    {
        public static ProductDto ToProductDto(this Product product, bool includeSales = true)
        {
            return new ProductDto()
            {
                Description = product.Description,
                Id = product.Id,
                ProductName = product.ProductName,
                Barcode = product.Barcode,
                BrandId = product.BrandId,
                Location = product.Location,
                Price = product.Price,
                ProductImage = product.ProductImage,
                Sales = includeSales ? product.Sales.Select(i => i.ToSalesDto(false)) : [],
                ShelfId = product.ShelfId,
                CreatedAt = product.CreatedAt,
                SoftDeletedAt = product.SoftDeletedAt,
                SoftDeleted = product.SoftDeleted,
                Stock = product.Stock,
                UpdatedAt = product.UpdatedAt,
                SalesTags = product.SalesTags,
            };
        }
        public static Product ToProductEntity(this CreateProductDto dto)
        {
            return new Product()
            {
                Description = dto.Description,
                Id = Guid.NewGuid(),
                ProductName = dto.ProductName,
                Barcode = dto.Barcode,
                Price = dto.Price,
                BrandId = dto.BrandId,
                Location = dto.Location,
                ProductImage = dto.ProductImage,
                SalesTags = dto.SalesTags,
                ShelfId = dto.ShelfId,
                Stock = dto.Stock,
            };
        }
        public static void ToProductEntity(this Product entity, UpdateProductDto dto)
        {
            entity.Description = dto.Description;
            entity.Location = dto.Location;
            entity.ProductName = dto.ProductName;
            entity.Barcode = dto.Barcode;
            entity.BrandId = dto.BrandId;
            entity.Price = dto.Price;
            entity.ProductImage = dto.ProductImage;
            entity.SalesTags = dto.SalesTags;
            entity.ShelfId = dto.ShelfId;
            entity.Stock = dto.Stock;
        }
        public static ProductBulkViewDto ToProductBulkViewDto(this Product product)
        {
            return new ProductBulkViewDto()
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Barcode = product.Barcode,
                Location = $"{product.Shelf.ShelfLocation}-{product.Location}",
                Price = product.Price,
                Sale = product.Sales.Any(),
                Stock = product.Stock,
                BrandName = product.Brand.BrandName,
            };
        }
    }
}
