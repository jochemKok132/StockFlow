using System.ComponentModel.DataAnnotations;

namespace StockFlow.Infrastructure.Receipts
{
    public class ReceiptOptions
    {
        public const string SectionName = "Receipt";

        [Required]
        [MaxLength(60)]
        public string StoreName { get; set; } = "StockFlow";

        public List<string> AddressLines { get; set; } = [];

        [Range(0, 100)]
        public decimal VatPercentage { get; set; } = 21;

        [Required]
        public string TimeZone { get; set; } = "Europe/Amsterdam";
    }
}
