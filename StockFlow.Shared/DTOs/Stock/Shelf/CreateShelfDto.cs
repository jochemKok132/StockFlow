using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Shelf
{
    public class CreateShelfDto
    {
        public string? ShelfName { get; set; }
        public string? ShelfHallway { get; set; }
        public string? ShelfLocation { get; set; }
    }
}
