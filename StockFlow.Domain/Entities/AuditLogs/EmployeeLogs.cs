using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.AuditLogs
{
    public class EmployeeLogs : IEntity, ICreateable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public LogType LogType { get; set; }
        public string Identifier { get; set; }
        public IEnumerable<string> Messages { get; set; } = [];
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
