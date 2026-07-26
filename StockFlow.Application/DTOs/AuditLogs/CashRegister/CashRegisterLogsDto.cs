using StockFlow.Application.DTOs.Customer;
using StockFlow.Application.DTOs.Employee;
using StockFlow.Application.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.AuditLogs.CashRegister
{
    public class CashRegisterLogsDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid EmployeeId { get; set; }
        public required EmployeeDto Employee { get; set; }
        public Guid? CustomerId { get; set; }
        public required CustomerDto Customer { get; set; }
        public double TotalPrice { get; set; }
        public double TotalOff { get; set; }
        public IEnumerable<ProductDto> ProductsSold { get; set; } = [];
    }
}
