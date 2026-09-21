using StockFlow.Application.DTOs.Employee;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.AuditLogs.Employee
{
    public class EmployeeLogsDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public LogTypeDto LogType { get; set; }
        public string Identifier { get; set; }
        public List<string> Messages { get; set; } = [];
        public Guid EmployeeId { get; set; }
        public EmployeeDto Employee { get; set; }
    }
}
