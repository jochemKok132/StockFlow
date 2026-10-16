using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Shelf
{
    public class CreateShelfDto
    {
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? ShelfName { get; set; }
        [MaxLength(FieldLimits.ShelfHallway, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? ShelfHallway { get; set; }
        [MaxLength(FieldLimits.ShelfLocation, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? ShelfLocation { get; set; }
    }
}
