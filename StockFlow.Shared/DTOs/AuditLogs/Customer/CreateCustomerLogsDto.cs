using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.AuditLogs.Customer
{
    public class CreateCustomerLogsDto
    {
        public Guid Id { get; set; }
        public LogTypeDto LogType { get; set; }
        public Guid CustomerId { get; set; }
        public List<string> Messages { get; set; } = [];
    }
}
