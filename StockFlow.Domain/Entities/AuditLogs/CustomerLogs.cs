using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.AuditLogs
{
    public class CustomerLogs : IEntity, ICreateable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public LogType LogType { get; set; }
        public Guid CustomerId { get; set; }
        public required Customer Customer { get; set; }
    }
}
