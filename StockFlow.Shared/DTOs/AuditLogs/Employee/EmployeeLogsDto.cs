using StockFlow.Shared.DTOs.Employee;
using StockFlow.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.AuditLogs.Employee
{
    public class EmployeeLogsDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public LogTypeDto LogType { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public List<string> Messages { get; set; } = [];
        public Guid EmployeeId { get; set; }
        public EmployeeDto Employee { get; set; } = null!;
    }
}
