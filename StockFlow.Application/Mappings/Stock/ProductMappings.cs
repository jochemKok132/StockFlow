using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.Stock
{
    public static class ProductMappings
    {
        public static ProductDto ToProductDto(this Product product)
        {
            return new ProductDto()
            {
                Description = product.Description,
                Id = product.Id,
                ProductName = product.ProductName,
                Barcode = product.Barcode,
                BrandId = product.BrandId,
                Location = product.Location,
                ProductImage = product.ProductImage,
                SaleId = product.SaleId,
                ShelfId = product.ShelfId,
                CreatedAt = product.CreatedAt,
                SoftDeletedAt = product.SoftDeletedAt,
                SoftDeleted = product.SoftDeleted,
                Stock = product.Stock,
                UpdatedAt = product.UpdatedAt,
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
                BrandId = dto.BrandId,
                Location = dto.Location,
                ProductImage = dto.ProductImage,
                SaleId = dto.SaleId,
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
            entity.ProductImage = dto.ProductImage;
            entity.SaleId = dto.SaleId;
            entity.ShelfId = dto.ShelfId;
            entity.Stock = dto.Stock;
        }
    }
}
