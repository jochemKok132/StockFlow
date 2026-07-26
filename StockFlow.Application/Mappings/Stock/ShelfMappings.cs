using StockFlow.Application.DTOs.Stock.Shelf;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.Stock
{
    public static class ShelfMappings
    {
        public static ShelfDto ToShelfDto(this Shelf shelf)
        {
            return new ShelfDto()
            {
                ShelfLocation = shelf.ShelfLocation,
                ShelfName = shelf.ShelfName,
                ShelfDescription = shelf.ShelfDescription,
                CreatedAt = shelf.CreatedAt,
                SoftDeleted = shelf.SoftDeleted,
                Id = shelf.Id,
                Products = shelf.Products.Select(p => p.ToProductDto()),
                SoftDeletedAt = shelf.SoftDeletedAt,
                UpdatedAt = shelf.UpdatedAt,
            };
        }
        public static Shelf ToShelfEntity(this CreateShelfDto dto)
        {
            return new Shelf()
            {
                ShelfLocation = dto.ShelfLocation,
                ShelfName = dto.ShelfName,
                Id = Guid.NewGuid(),
                ShelfDescription = dto.ShelfDescription,
            };
        }
        public static void ToShelfEntity(this Shelf entity, UpdateShelfDto dto)
        {
            entity.ShelfDescription = dto.ShelfDescription;
            entity.ShelfLocation = dto.ShelfLocation;
            entity.ShelfName = dto.ShelfName;
        }
    }
}
