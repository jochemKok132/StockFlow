using StockFlow.Domain.Entities.IEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.People
{
    public class Customer : IEntity, ICreateable, IUpdatable, ISoftDeletable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }
        public required string CustomerId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? PostalCode { get; set; }
        public string? HouseNumber { get; set; }
        public required string HashedPassword { get; set; }
        public int SavedPoints { get; set; } = 0;
    }
}
