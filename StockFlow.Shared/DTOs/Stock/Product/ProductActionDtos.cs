using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;

namespace StockFlow.Shared.DTOs.Stock.Product
{
    public class ProductPriceDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByName { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsFuture { get; set; }
    }

    public class StockBatchDto
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

    public class SchedulePriceRequest
    {
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
    }

    public class RecallRequest
    {
        public Guid ProductId { get; set; }
        [MaxLength(FieldLimits.Reason, ErrorMessage = FieldLimits.TooLongMessage)]
        public string Reason { get; set; } = string.Empty;
        public bool WriteOffStock { get; set; }
    }

    public class OrderBlockRequest
    {
        public Guid ProductId { get; set; }
        [MaxLength(FieldLimits.Reason, ErrorMessage = FieldLimits.TooLongMessage)]
        public string Reason { get; set; } = string.Empty;
        public DateTime? Until { get; set; }
    }

    public class ProductSalesDto
    {
        public Guid ProductId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int ItemsSold { get; set; }
        public int ItemsReturned { get; set; }
        public decimal Revenue { get; set; }
        public List<ProductSalesDayDto> Days { get; set; } = [];
    }

    public class ProductSalesDayDto
    {
        public DateTime Date { get; set; }
        public int ItemsSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
