using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Employee;
using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.AuditLogs.CashRegister
{
    public class CreateCashRegisterLogsDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? CustomerId { get; set; }
        public double TotalPrice { get; set; }
        public double TotalOff { get; set; }
        public int PointsUsed { get; set; }
        public int PointsEarned { get; set; }
        public IEnumerable<RegisterItemDetailDto> ProductsSold { get; set; } = [];
    }
}
