using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.CashRegister;
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
                ReceiptId = log.ReceiptId,
                ProductsSold = log.ProductsSold.Select(p => p.ToRegisterItemDetailDto()),
                TotalOff = log.TotalOff,
                PointsUsed = log.PointsUsed,
                PointsEarned = log.PointsEarned,
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
                ProductsSold = dto.ProductsSold.ToList(),
                TotalOff = dto.TotalOff,
                PointsUsed = dto.PointsUsed,
                TotalPrice = dto.TotalPrice,
            };
        }
        public static CashRegisterLogs ToCashRegisterLogsEntity(this CreateCashRegisterLogsDto dto, string receiptId)
        {
            var newId = Guid.NewGuid();
            return new CashRegisterLogs()
            {
                ReceiptId = receiptId,
                CustomerId = dto.CustomerId,
                EmployeeId = dto.EmployeeId,
                Id = newId,
                ProductsSold = dto.ProductsSold
                    .Select(e =>
                    {
                        var entity = e.ToRegisterItemDetailEntity();
                        entity.CashregisterLogId = newId;
                        return entity;
                    })
                    .ToList(),
                TotalOff = dto.TotalOff,
                PointsUsed = dto.PointsUsed,
                PointsEarned = dto.PointsEarned,
                TotalPrice = dto.TotalPrice,
            };
        }
    }
}
