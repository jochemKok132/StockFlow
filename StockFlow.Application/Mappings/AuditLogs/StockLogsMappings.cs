using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Mappings.People;
using StockFlow.Domain.Entities.AuditLogs;
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
                LogType = log.LogType.ToDto(),
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
                StockLogType = dto.StockLogType.ToDomain(),
            };
        }
    }
}
