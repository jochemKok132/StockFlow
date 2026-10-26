using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;

namespace StockFlow.Shared.DTOs.Stock.Supplier
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public DayOfWeek OrderDay { get; set; }
        public int LeadTimeDays { get; set; }
        public DateTime NextOrderDate { get; set; }
        public int ProductCount { get; set; }
    }

    public class CreateSupplierDto
    {
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public string SupplierName { get; set; } = string.Empty;
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? ContactName { get; set; }
        [MaxLength(FieldLimits.Email, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? Email { get; set; }
        [MaxLength(FieldLimits.Phone, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? Phone { get; set; }
        [MaxLength(FieldLimits.Url, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? Website { get; set; }
        public DayOfWeek OrderDay { get; set; } = DayOfWeek.Monday;
        public int LeadTimeDays { get; set; } = 2;
    }

    public class UpdateSupplierDto : CreateSupplierDto
    {
        public Guid Id { get; set; }
    }
}
