namespace StockFlow.Shared.DTOs.Releases
{
    public class ReleaseDto
    {
        public string TagName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
        public bool Prerelease { get; set; }
        public string Url { get; set; } = string.Empty;
        public List<ReleaseAssetDto> Assets { get; set; } = [];
        public ReleaseAssetDto? CashRegister { get; set; }
    }

    public class ReleaseAssetDto
    {
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
        public int DownloadCount { get; set; }
    }
}
