using StockFlow.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.AuditLogs.Employee
{
    public class CreateEmployeeLogsDto
    {
        public Guid Id { get; set; }
        public LogTypeDto LogType { get; set; }
        public string Identifier { get; set; }
        public List<string> Messages { get; set; } = [];
        public Guid EmployeeId { get; set; }
    }
}
