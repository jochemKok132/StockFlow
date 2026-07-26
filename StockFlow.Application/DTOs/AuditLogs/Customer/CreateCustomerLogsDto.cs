using StockFlow.Application.DTOs.Customer;
using StockFlow.Application.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.AuditLogs.Customer
{
    public class CreateCustomerLogsDto
    {
        public Guid Id { get; set; }
        public LogTypeDto LogType { get; set; }
        public Guid CustomerId { get; set; }
    }
}
