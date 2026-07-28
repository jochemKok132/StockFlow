using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.Mappings.People;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Domain.Entities.AuditLogs;
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
                Customer = log.Customer.ToCustomerDto(),
                Employee = log.Employee.ToEmployeeDto(),
                CreatedAt = log.CreatedAt,
                EmployeeId = log.EmployeeId,
                Id = log.Id,
                ProductsSold = log.ProductsSold.Select(p => p.ToProductDto()),
                TotalOff = log.TotalOff,
                TotalPrice = log.TotalPrice,
            };
        }
        public static CashRegisterLogs ToCashRegisterLogsEntity(this CreateCashRegisterLogsDto dto)
        {
            return new CashRegisterLogs()
            {
                CustomerId = dto.CustomerId,
                EmployeeId = dto.EmployeeId,
                Id = Guid.NewGuid(),
                ProductsSold = dto.ProductsSoldIds.Select(id => new Product { Id = id, Description = null!, ProductName = null! }),
                TotalOff = dto.TotalOff,
                TotalPrice = dto.TotalPrice,
            };
        }
    }
}
