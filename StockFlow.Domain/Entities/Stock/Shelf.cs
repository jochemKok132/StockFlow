using StockFlow.Domain.Entities.IEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.Stock
{
    public class Shelf : IEntity, ICreateable, IUpdatable, ISoftDeletable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }

        public required string ShelfName { get; set; }
        public string? ShelfHallway { get; set; }
        public required string ShelfLocation { get; set; }
        public ICollection<Product> Products { get; set; } = [];
    }
}
