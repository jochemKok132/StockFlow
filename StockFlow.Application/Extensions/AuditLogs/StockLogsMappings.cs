using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.ProductBrand;
using StockFlow.Application.DTOs.Stock.Sales;
using StockFlow.Application.DTOs.Stock.Shelf;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Mappings.People;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.AuditLogs
{
    public static class StockLogsMappings
    {
        public static StockLogsDto ToStockLogsDto(this StockLogs log)
        {
            return new StockLogsDto()
            {
                EmployeeId = log.EmployeeId,
                Employee = log.Employee.ToEmployeeDto(),
                CreatedAt = log.CreatedAt,
                Id = log.Id,
                Messages = log.Messages.ToList(),
                LogType = log.LogType.ToDto(),
                Identifier = log.Identifier,
                StockLogType = log.StockLogType.ToDto(),
            };
        }
        public static StockLogs ToStockLogsEntity(this CreateStockLogsDto dto)
        {
            return new StockLogs()
            {
                EmployeeId = dto.EmployeeId,
                Id = Guid.NewGuid(),
                LogType = dto.LogType.ToDomain(),
                Messages = dto.Messages,
                Identifier = dto.Identifier,
                StockLogType = dto.StockLogType.ToDomain(),
            };
        }
        public static CreateStockLogsDto ToCreateStockLogsDto(this CreateProductBrandDto dto, Guid userId)
        {
            return new CreateStockLogsDto()
            {
                Identifier = dto.BrandName.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Create,
                Messages = [],
                StockLogType = StockLogTypeDto.ProductBrand,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ToUpdateStockLogsDto(this UpdateProductBrandDto dto, ProductBrand entity, Guid userId)
        {
            return new CreateStockLogsDto()
            {
                Identifier = dto.BrandName.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Update,
                Messages = dto.FindDifferences(entity),
                StockLogType = StockLogTypeDto.ProductBrand,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ChangeStock(this Product entity, int amount, Guid userId)
        {
            var dto = entity.ToProductDto();
            dto.Stock += amount;
            return new CreateStockLogsDto()
            {
                Identifier = entity.Barcode.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Sold,
                Messages = dto.FindDifferences(entity),
                StockLogType = StockLogTypeDto.Product,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ToSoftDeleteStockLogsDto(this ProductBrand entity, Guid userId)
        {
            return new CreateStockLogsDto()
            {
                Identifier = entity.BrandName.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Delete,
                Messages = [],
                StockLogType = StockLogTypeDto.ProductBrand,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ToCreateStockLogsDto(this CreateProductDto dto, Guid userId, int barcode)
        {
            return new CreateStockLogsDto()
            {
                Identifier = barcode.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Create,
                Messages = [],
                StockLogType = StockLogTypeDto.Product,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ToUpdateStockLogsDto(this UpdateProductDto dto, Product entity, Guid userId)
        {
            return new CreateStockLogsDto()
            {
                Identifier = entity.Barcode.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Update,
                Messages = dto.FindDifferences(entity),
                StockLogType = StockLogTypeDto.Product,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ToSoftDeleteStockLogsDto(this Product entity, Guid userId)
        {
            return new CreateStockLogsDto()
            {
                Identifier = entity.Barcode.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Delete,
                Messages = [],
                StockLogType = StockLogTypeDto.Product,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ToCreateStockLogsDto(this CreateShelfDto dto, Guid userId)
        {
            return new CreateStockLogsDto()
            {
                Identifier = dto.ShelfName.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Create,
                Messages = [],
                StockLogType = StockLogTypeDto.Shelf,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ToUpdateStockLogsDto(this UpdateShelfDto dto, Shelf entity, Guid userId)
        {
            return new CreateStockLogsDto()
            {
                Identifier = dto.ShelfName.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Update,
                Messages = dto.FindDifferences(entity),
                StockLogType = StockLogTypeDto.Shelf,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ToSoftDeleteStockLogsDto(this Shelf entity, Guid userId)
        {
            return new CreateStockLogsDto()
            {
                Identifier = entity.ShelfName.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Delete,
                Messages = [],
                StockLogType = StockLogTypeDto.Shelf,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ToCreateStockLogsDto(this CreateSalesDto dto, Guid userId)
        {
            return new CreateStockLogsDto()
            {
                Identifier = dto.SaleName.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Create,
                Messages = [],
                StockLogType = StockLogTypeDto.Sales,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ToUpdateStockLogsDto(this UpdateSalesDto dto, Sales entity, Guid userId)
        {
            return new CreateStockLogsDto()
            {
                Identifier = dto.SaleName.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Update,
                Messages = dto.FindDifferences(entity),
                StockLogType = StockLogTypeDto.Sales,
                EmployeeId = userId,
            };
        }
        public static CreateStockLogsDto ToSoftDeleteStockLogsDto(this Sales entity, Guid userId)
        {
            return new CreateStockLogsDto()
            {
                Identifier = entity.SaleName.ToString(),
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Delete,
                Messages = [],
                StockLogType = StockLogTypeDto.Sales,
                EmployeeId = userId,
            };
        }
    }
}
