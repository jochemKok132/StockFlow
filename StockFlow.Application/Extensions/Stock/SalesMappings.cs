using StockFlow.Shared.DTOs.Stock.Sales;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.Stock
{
    public static class SalesMappings
    {
        public static SalesDto ToSalesDto(this Sales sales, bool includeProducts = true)
        {
            return new SalesDto()
            {
                Description = sales.Description,
                SaleName = sales.SaleName,
                CreatedAt = sales.CreatedAt,
                Id = sales.Id,
                SaleType = sales.SaleType.ToDto(),
                PercentageOff = sales.PercentageOff,
                BuyAmount = sales.BuyAmount,
                FreeAmount = sales.FreeAmount,
                BundlePrice = sales.BundlePrice,
                StartDate = sales.StartDate,
                EndDate = sales.EndDate,
                SalesTags = sales.SalesTags,
                SoftDeleted = sales.SoftDeleted,
                SoftDeletedAt = sales.SoftDeletedAt,
                UpdatedAt = sales.UpdatedAt,
                Products = includeProducts ? sales.Products.Select(x => x.ToProductDto(false)) : [],
            };
        }
        public static Sales ToSalesEntity(this CreateSalesDto dto)
        {
            var entity = new Sales()
            {
                Description = dto.Description,
                SaleName = dto.SaleName,
                Id = Guid.NewGuid(),
                SalesTags = dto.SalesTags,
                StartDate = dto.StartDate?.Date,
                EndDate = dto.EndDate?.Date,
            };
            entity.ApplyType(dto.SaleType, dto.PercentageOff, dto.BuyAmount, dto.FreeAmount, dto.BundlePrice);
            return entity;
        }

        private static void ApplyType(this Sales entity, SaleTypeDto type, int percentageOff, int buyAmount, int freeAmount, decimal bundlePrice)
        {
            entity.SaleType = type.ToDomain();
            entity.PercentageOff = type is SaleTypeDto.Percentage or SaleTypeDto.Moving ? percentageOff : 0;
            entity.BuyAmount = type is SaleTypeDto.MultiBuy or SaleTypeDto.Bundle ? buyAmount : 0;
            entity.FreeAmount = type == SaleTypeDto.MultiBuy ? freeAmount : 0;
            entity.BundlePrice = type == SaleTypeDto.Bundle ? bundlePrice : 0;
        }
        public static void ToSalesEntity(this Sales entity, UpdateSalesDto dto)
        {
            entity.SaleName = dto.SaleName;
            entity.Description = dto.Description;
            entity.StartDate = dto.StartDate?.Date;
            entity.EndDate = dto.EndDate?.Date;

            if (entity.SaleType == Domain.Enums.SaleType.Moving)
            {
                entity.PercentageOff = dto.PercentageOff;
                return;
            }

            entity.ApplyType(dto.SaleType, dto.PercentageOff, dto.BuyAmount, dto.FreeAmount, dto.BundlePrice);
            entity.SalesTags = dto.SalesTags;
        }
    }
}
