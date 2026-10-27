using StockFlow.Domain.Entities.Scheduling;
using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.People
{
    public class Employee : IEntity, ICreateable, IUpdatable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required string EmployeeId { get; set; }
        public required string FullName { get; set; }
        public string? HashedPassword { get; set; }
        public EmployeeRole Role { get; set; } = EmployeeRole.None;
        public int ContractHours { get; set; }
        public ICollection<EmployeeAvailability> Availability { get; set; } = [];
    }
}
