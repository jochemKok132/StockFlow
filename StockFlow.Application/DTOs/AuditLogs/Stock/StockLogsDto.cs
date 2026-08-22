using StockFlow.Application.DTOs.Employee;
using StockFlow.Application.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.AuditLogs.Stock
{
    public class StockLogsDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Barcode { get; set; }
        public StockLogTypeDto StockLogType { get; set; }
        public LogTypeDto LogType { get; set; }
        public Guid EmployeeId { get; set; }
        public required EmployeeDto Employee { get; set; }
    }
}
