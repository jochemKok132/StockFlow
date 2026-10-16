using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Shelf
{
    public class UpdateShelfDto
    {
        public Guid Id { get; set; }
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public required string ShelfName { get; set; }
        [MaxLength(FieldLimits.ShelfHallway, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? ShelfHallway { get; set; }
        [MaxLength(FieldLimits.ShelfLocation, ErrorMessage = FieldLimits.TooLongMessage)]
        public required string ShelfLocation { get; set; }
    }
}
