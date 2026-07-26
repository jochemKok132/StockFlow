using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Stock.Shelf
{
    public class UpdateShelfDto
    {
        public Guid Id { get; set; }
        public required string ShelfName { get; set; }
        public string? ShelfDescription { get; set; }
        public required string ShelfLocation { get; set; }
    }
}
