using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.Logs
{
    public class CashRegisterLogs : IEntity, ICreateable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid EmployeeId { get; set; }
        public required Employee Employee { get; set; }
        public Guid? CustomerId { get; set; }
        public required Customer Customer{ get; set; }
        public double TotalPrice { get; set; }
        public double TotalOff { get; set; }
        public IEnumerable<Product> ProductsSold { get; set; } = [];
    }
}
