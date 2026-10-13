using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using StockFlow.Application.Interfaces;
using StockFlow.Shared.DTOs.Releases;

namespace StockFlow.Infrastructure.Releases
{
    public class GitHubReleaseService(HttpClient httpClient, ReleaseCache cache, IOptions<GitHubReleasesOptions> options) : IReleaseService
    {
        private const int PageSize = 30;

        public async Task<List<ReleaseDto>> GetReleasesAsync()
        {
            if (cache.TryGet(out var cached))
                return cached;

            try
            {
                var releases = await httpClient.GetFromJsonAsync<List<GitHubRelease>>(
                    $"repos/{options.Value.Repository}/releases?per_page={PageSize}") ?? [];

                var result = releases
                    .Where(r => !r.Draft)
                    .Select(ToDto)
                    .OrderByDescending(r => r.PublishedAt)
                    .ToList();

                cache.Set(result, TimeSpan.FromMinutes(options.Value.CacheMinutes));
                return result;
            }
            catch (Exception exception) when (exception is HttpRequestException or JsonException or TaskCanceledException)
            {
                if (cache.Stale is { } stale)
                    return stale;

                throw new HttpRequestException("The releases could not be loaded from GitHub right now.", exception);
            }
        }

        public async Task<ReleaseDto?> GetLatestReleaseAsync() =>
            (await GetReleasesAsync()).FirstOrDefault(r => !r.Prerelease);

        private static ReleaseDto ToDto(GitHubRelease release)
        {
            var assets = release.Assets
                .Select(a => new ReleaseAssetDto
                {
                    Name = a.Name,
                    Size = a.Size,
                    DownloadUrl = a.BrowserDownloadUrl,
                    DownloadCount = a.DownloadCount,
                })
                .ToList();

            return new ReleaseDto
            {
                TagName = release.TagName,
                Version = release.TagName.TrimStart('v', 'V'),
                Name = string.IsNullOrWhiteSpace(release.Name) ? release.TagName : release.Name,
                Notes = release.Body ?? string.Empty,
                PublishedAt = release.PublishedAt,
                Prerelease = release.Prerelease,
                Url = release.HtmlUrl,
                Assets = assets,
                CashRegister = assets.FirstOrDefault(a =>
                    a.Name.StartsWith(GitHubReleasesOptions.CashRegisterAssetPrefix, StringComparison.OrdinalIgnoreCase) &&
                    a.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)),
            };
        }

        private class GitHubRelease
        {
            [JsonPropertyName("tag_name")] public string TagName { get; set; } = string.Empty;
            [JsonPropertyName("name")] public string? Name { get; set; }
            [JsonPropertyName("body")] public string? Body { get; set; }
            [JsonPropertyName("draft")] public bool Draft { get; set; }
            [JsonPropertyName("prerelease")] public bool Prerelease { get; set; }
            [JsonPropertyName("published_at")] public DateTime? PublishedAt { get; set; }
            [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = string.Empty;
            [JsonPropertyName("assets")] public List<GitHubAsset> Assets { get; set; } = [];
        }

        private class GitHubAsset
        {
            [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
            [JsonPropertyName("size")] public long Size { get; set; }
            [JsonPropertyName("browser_download_url")] public string BrowserDownloadUrl { get; set; } = string.Empty;
            [JsonPropertyName("download_count")] public int DownloadCount { get; set; }
        }
    }
}
