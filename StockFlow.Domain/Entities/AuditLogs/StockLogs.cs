using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.AuditLogs
{
    public class StockLogs : IEntity, ICreateable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public StockLogType StockLogType { get; set; }
        public LogType LogType { get; set; }
        public Guid EmployeeId { get; set; }
        public required Employee Employee { get; set; }
    }
}
