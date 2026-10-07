using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.CashRegister;
using StockFlow.Application.Extensions.CashRegister;
using StockFlow.Application.Mappings.People;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.CashRegister;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.AuditLogs
{
    public static class CashRegisterLogsMappings
    {
        public static CashRegisterLogsDto ToCashRegisterLogsDto(this CashRegisterLogs log)
        {
            return new CashRegisterLogsDto()
            {
                CustomerId = log.CustomerId,
                Customer = log.Customer?.ToCustomerDto(),
                Employee = log.Employee?.ToEmployeeDto(),
                CreatedAt = log.CreatedAt,
                EmployeeId = log.EmployeeId,
                Id = log.Id,
                ProductsSold = log.ProductsSold.Select(p => p.ToRegisterItemDetailDto()),
                TotalOff = log.TotalOff,
                TotalPrice = log.TotalPrice,
            };
        }
        public static CreateCashRegisterLogsDto ToCashRegisterLogsDto(this Purchase dto, Guid employeeId)
        {
            return new CreateCashRegisterLogsDto()
            {
                CustomerId = dto.CustomerId,
                EmployeeId = employeeId,
                Id = Guid.NewGuid(),
                ProductsSold = dto.ProductsSold.Select(e => e.ToRegisterItemDetailEntity()),
                TotalOff = dto.TotalOff,
                TotalPrice = dto.TotalPrice,
            };
        }
        public static CashRegisterLogs ToCashRegisterLogsEntity(this CreateCashRegisterLogsDto dto)
        {
            var newId = Guid.NewGuid();
            return new CashRegisterLogs()
            {
                CustomerId = dto.CustomerId,
                EmployeeId = dto.EmployeeId,
                Id = newId,
                ProductsSold = dto.ProductsSold
                    .Select(e =>
                    {
                        e.CashregisterLogId = newId;
                        return e;
                    })
                    .ToList(),
                TotalOff = dto.TotalOff,
                TotalPrice = dto.TotalPrice,
            };
        }
    }
}
