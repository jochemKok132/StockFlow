using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Stock.Shelf
{
    public class CreateShelfDto
    {
        public required string ShelfName { get; set; }
        public string? ShelfDescription { get; set; }
        public required string ShelfLocation { get; set; }
    }
}
