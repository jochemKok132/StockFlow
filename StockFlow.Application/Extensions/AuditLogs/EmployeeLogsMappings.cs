using StockFlow.Shared.DTOs.AuditLogs.Employee;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Employee;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Stock.ProductBrand;
using StockFlow.Shared.DTOs.Stock.Sales;
using StockFlow.Shared.DTOs.Stock.Shelf;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Mappings.People;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.AuditLogs
{
    public static class EmployeeLogsMappings
    {
        public static EmployeeLogsDto ToEmployeeLogsDto(this EmployeeLogs log)
        {
            return new EmployeeLogsDto()
            {
                EmployeeId = log.EmployeeId,
                Employee = log.Employee.ToEmployeeDto(),
                CreatedAt = log.CreatedAt,
                Id = log.Id,
                Messages = log.Messages.WithoutNavigationChanges(),
                LogType = log.LogType.ToDto(),
                Identifier = log.Identifier,
            };
        }
        public static EmployeeLogs ToEmployeeLogsEntity(this CreateEmployeeLogsDto dto)
        {
            return new EmployeeLogs()
            {
                EmployeeId = dto.EmployeeId,
                Id = Guid.NewGuid(),
                LogType = dto.LogType.ToDomain(),
                Messages = dto.Messages,
                Identifier = dto.Identifier,
            };
        }
        public static CreateEmployeeLogsDto ToCreateEmployeeLogsDto(this CreateEmployeeDto dto, string employeeId, Guid userId)
        {
            return new CreateEmployeeLogsDto()
            {
                Identifier = employeeId,
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Create,
                Messages = [],
                EmployeeId = userId,
            };
        }
        public static CreateEmployeeLogsDto ToUpdateEmployeeLogsDto(this UpdateEmployeeDto dto, Employee entity, Guid userId)
        {
            return new CreateEmployeeLogsDto()
            {
                Identifier = entity.EmployeeId,
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Update,
                Messages = dto.FindDifferences(entity),
                EmployeeId = userId,
            };
        }
        public static CreateEmployeeLogsDto ToLoginEmployeeLogsDto(this Employee entity)
        {
            return new CreateEmployeeLogsDto()
            {
                Identifier = entity.EmployeeId,
                Id = Guid.NewGuid(),
                LogType = LogTypeDto.Login,
                Messages = [],
                EmployeeId = entity.Id,
            };
        }
    }
}
