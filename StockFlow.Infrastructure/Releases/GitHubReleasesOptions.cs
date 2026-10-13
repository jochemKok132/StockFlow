using System.ComponentModel.DataAnnotations;

namespace StockFlow.Infrastructure.Releases
{
    public class GitHubReleasesOptions
    {
        public const string SectionName = "Releases";
        public const string CashRegisterAssetPrefix = "StockFlow.CashRegister";

        [Required]
        [RegularExpression(@"^[\w.-]+/[\w.-]+$", ErrorMessage = "Releases:Repository must look like owner/repository.")]
        public string Repository { get; set; } = string.Empty;

        public string? Token { get; set; }

        [Range(1, 1440)]
        public int CacheMinutes { get; set; } = 10;
    }
}
