using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Shared.DTOs.Ordering
{
    public class StockCountDto
    {
        public Guid Id { get; set; }
        public int CountNumber { get; set; }
        public StockCountTypeDto Type { get; set; }
        public StockCountStatusDto Status { get; set; }
        public Guid ShelfId { get; set; }
        public string ShelfName { get; set; } = string.Empty;
        public string ShelfLocation { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CompletedByName { get; set; }
        public List<StockCountLineDto> Lines { get; set; } = [];
    }

    public class StockCountLineDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Barcode { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool CanExpire { get; set; }
        public DateTime? FirstExpiryDate { get; set; }
        public int ExpectedStock { get; set; }
        public int? CountedStock { get; set; }
        public int ExpiredAmount { get; set; }
        public DateTime? NextExpiryDate { get; set; }
    }

    public class CreateStockCountRequest
    {
        public Guid ShelfId { get; set; }
        [MaxLength(FieldLimits.OrderNote, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? Note { get; set; }
        public bool ExpirableOnly { get; set; }
    }

    public class CompleteStockCountRequest
    {
        public List<StockCountEntry> Lines { get; set; } = [];
    }

    public class StockCountEntry
    {
        public Guid LineId { get; set; }
        public int CountedStock { get; set; }
        public int ExpiredAmount { get; set; }
        public DateTime? NextExpiryDate { get; set; }
    }
}
