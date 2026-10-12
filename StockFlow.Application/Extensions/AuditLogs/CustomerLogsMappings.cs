using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.DTOs.Customer;
using StockFlow.Application.DTOs.Enums;
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
                CustomerId = log.CustomerId,
                Id = log.Id,
                LogType = log.LogType.ToDto(),
                Messages = log.Messages.WithoutNavigationChanges(),
            };
        }
        public static CustomerLogs ToCustomerLogsEntity(this CreateCustomerLogsDto dto)
        {
            return new CustomerLogs()
            {
                CustomerId = dto.CustomerId,
                Id = Guid.NewGuid(),
                LogType = dto.LogType.ToDomain(),
                Messages = dto.Messages,
            };
        }
        public static CreateCustomerLogsDto ToUpdateCustomerLogsDto(this UpdateCustomerDto dto, StockFlow.Domain.Entities.People.Customer entity)
        {
            return new CreateCustomerLogsDto()
            {
                Id = Guid.NewGuid(),
                CustomerId = entity.Id,
                LogType = LogTypeDto.Update,
                Messages = dto.FindDifferences(entity),
            };
        }
    }
}
