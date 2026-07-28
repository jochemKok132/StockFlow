using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Mappings.People;
using StockFlow.Domain.Entities.AuditLogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.AuditLogs
{
    public static class CustomerLogsMappings
    {
        public static CustomerLogsDto ToCustomerLogsDto(this CustomerLogs log)
        {
            return new CustomerLogsDto()
            {
                Customer = log.Customer.ToCustomerDto(),
                CreatedAt = log.CreatedAt,
                CustomerId = log.Customer.Id,
                Id = log.Customer.Id,
                LogType = log.LogType.ToDto(),
            };
        }
        public static CustomerLogs ToCustomerLogsEntity(this CreateCustomerLogsDto dto)
        {
            return new CustomerLogs()
            {
                CustomerId = dto.CustomerId,
                Id = Guid.NewGuid(),
                LogType = dto.LogType.ToDomain(),
            };
        }
    }
}
