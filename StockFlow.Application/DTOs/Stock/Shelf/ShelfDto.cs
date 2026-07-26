using StockFlow.Application.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Stock.Shelf
{
    public class ShelfDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }

        public required string ShelfName { get; set; }
        public string? ShelfDescription { get; set; }
        public required string ShelfLocation { get; set; }
        public IEnumerable<ProductDto> Products { get; set; } = [];
    }
}
