using System.Net;
using Microsoft.Extensions.Options;
using StockFlow.Infrastructure.Releases;

namespace StockFlow.UnitTests.Infrastructure
{
    public class GitHubReleaseServiceTests
    {
        private const string ReleasesJson = """
            [
              {
                "tag_name": "v1.3.0-beta.1", "name": "Beta", "body": "Testing", "draft": false, "prerelease": true,
                "published_at": "2026-10-12T10:00:00Z", "html_url": "https://github.com/x/y/releases/tag/v1.3.0-beta.1", "assets": []
              },
              {
                "tag_name": "v1.2.0", "name": "", "body": "## Changes\n- Returns", "draft": false, "prerelease": false,
                "published_at": "2026-10-10T10:00:00Z", "html_url": "https://github.com/x/y/releases/tag/v1.2.0",
                "assets": [
                  { "name": "StockFlow.CashRegister-v1.2.0-win-x64.zip", "size": 73400320, "browser_download_url": "https://github.com/x/y/releases/download/v1.2.0/cashregister.zip", "download_count": 7 },
                  { "name": "notes.txt", "size": 12, "browser_download_url": "https://github.com/x/y/notes.txt", "download_count": 1 }
                ]
              },
              {
                "tag_name": "v1.4.0", "name": "Draft", "body": "", "draft": true, "prerelease": false,
                "published_at": null, "html_url": "", "assets": []
              }
            ]
            """;

        private class FakeHandler(HttpStatusCode status, string body) : HttpMessageHandler
        {
            public int Calls { get; private set; }
            public HttpStatusCode Status { get; set; } = status;
            public string? LastPath { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Calls++;
                LastPath = request.RequestUri?.PathAndQuery;
                return Task.FromResult(new HttpResponseMessage(Status) { Content = new StringContent(body) });
            }
        }

        private static (GitHubReleaseService sut, FakeHandler handler, ReleaseCache cache) Create(HttpStatusCode status = HttpStatusCode.OK, string body = ReleasesJson)
        {
            var handler = new FakeHandler(status, body);
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.github.com/") };
            var cache = new ReleaseCache();
            var options = Options.Create(new GitHubReleasesOptions { Repository = "jochemKok132/StockFlow", CacheMinutes = 10 });
            return (new GitHubReleaseService(client, cache, options), handler, cache);
        }

        [Fact]
        public async Task GetReleasesAsync_GitHubResponse_SkipsDraftsAndSortsNewestFirst()
        {
            var (sut, handler, _) = Create();

            var result = await sut.GetReleasesAsync();

            Assert.Equal(["v1.3.0-beta.1", "v1.2.0"], result.Select(r => r.TagName));
            Assert.Equal("/repos/jochemKok132/StockFlow/releases?per_page=30", handler.LastPath);
        }

        [Fact]
        public async Task GetReleasesAsync_Release_MapsVersionNameAndCashRegisterAsset()
        {
            var (sut, _, _) = Create();

            var release = (await sut.GetReleasesAsync()).Single(r => r.TagName == "v1.2.0");

            Assert.Equal("1.2.0", release.Version);
            Assert.Equal("v1.2.0", release.Name);
            Assert.Equal(2, release.Assets.Count);
            Assert.NotNull(release.CashRegister);
            Assert.Equal("StockFlow.CashRegister-v1.2.0-win-x64.zip", release.CashRegister.Name);
            Assert.Equal(7, release.CashRegister.DownloadCount);
        }

        [Fact]
        public async Task GetLatestReleaseAsync_PrereleaseIsNewer_ReturnsNewestStableRelease()
        {
            var (sut, _, _) = Create();

            var latest = await sut.GetLatestReleaseAsync();

            Assert.Equal("v1.2.0", latest?.TagName);
        }

        [Fact]
        public async Task GetReleasesAsync_CalledTwice_UsesCache()
        {
            var (sut, handler, _) = Create();

            await sut.GetReleasesAsync();
            await sut.GetReleasesAsync();

            Assert.Equal(1, handler.Calls);
        }

        [Fact]
        public async Task GetReleasesAsync_GitHubFailsAfterCacheExpired_ReturnsStaleReleases()
        {
            var (sut, handler, cache) = Create();
            await sut.GetReleasesAsync();
            cache.Invalidate();
            handler.Status = HttpStatusCode.InternalServerError;

            var result = await sut.GetReleasesAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal(2, handler.Calls);
        }

        [Fact]
        public async Task GetReleasesAsync_GitHubUnavailableWithoutCache_ThrowsHttpRequestException()
        {
            var (sut, _, _) = Create(HttpStatusCode.NotFound, "{}");

            await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetReleasesAsync());
        }

        [Fact]
        public async Task GetReleasesAsync_InvalidJson_ThrowsHttpRequestException()
        {
            var (sut, _, _) = Create(body: "not json");

            await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetReleasesAsync());
        }
    }
}
