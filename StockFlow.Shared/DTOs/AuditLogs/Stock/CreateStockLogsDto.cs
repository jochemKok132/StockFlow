using StockFlow.Shared.DTOs.Employee;
using StockFlow.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.AuditLogs.Stock
{
    public class CreateStockLogsDto
    {
        public Guid Id { get; set; }
        public StockLogTypeDto StockLogType { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public LogTypeDto LogType { get; set; }
        public List<string> Messages { get; set; } = [];
        public Guid EmployeeId { get; set; }
    }
}
