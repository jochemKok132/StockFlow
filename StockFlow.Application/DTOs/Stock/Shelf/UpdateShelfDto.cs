using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Stock.Shelf
{
    public class UpdateShelfDto
    {
        public Guid Id { get; set; }
        public required string ShelfName { get; set; }
        public string? ShelfHallway { get; set; }
        public required string ShelfLocation { get; set; }
    }
}
