using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Shelf
{
    public class UpdateShelfDto
    {
        public Guid Id { get; set; }
        public required string ShelfName { get; set; }
        public string? ShelfHallway { get; set; }
        public required string ShelfLocation { get; set; }
    }
}
