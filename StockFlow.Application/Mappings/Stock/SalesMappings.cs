using StockFlow.Application.DTOs.Stock.Sales;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.Stock
{
    public static class SalesMappings
    {
        public static SalesDto ToSalesDto(this Sales sales)
        {
            return new SalesDto()
            {
                Description = sales.Description,
                SaleName = sales.SaleName,
                CreatedAt = sales.CreatedAt,
                Id = sales.Id,
                PercentageOff = sales.PercentageOff,
                Products = sales.Products.Select(p => p.ToProductDto()),
                SoftDeleted = sales.SoftDeleted,
                SoftDeletedAt = sales.SoftDeletedAt,
                UpdatedAt = sales.UpdatedAt,
            };
        }
        public static Sales ToSalesEntity(this CreateSalesDto dto)
        {
            return new Sales()
            {
                Description = dto.Description,
                SaleName = dto.SaleName,
                Id = Guid.NewGuid(),
                PercentageOff = dto.PercentageOff,
            };
        }
        public static void ToSalesEntity(this Sales entity, UpdateSalesDto dto)
        {
            entity.SaleName = dto.SaleName;
            entity.PercentageOff = dto.PercentageOff;
            entity.Description = dto.Description;
        }
    }
}
