using Microsoft.AspNetCore.Http;
using StockFlow.API.Controllers;
using StockFlow.Shared.DTOs.Releases;

namespace StockFlow.UnitTests.Api
{
    public class ReleasesControllerTests
    {
        private readonly Mock<IReleaseService> _releaseService = new();

        [Fact]
        public async Task GetAllAsync_GitHubDown_Returns503()
        {
            _releaseService.Setup(s => s.GetReleasesAsync()).ThrowsAsync(new HttpRequestException("down"));
            var sut = new ReleasesController(_releaseService.Object).WithUser(Guid.NewGuid());

            var result = await sut.GetAllAsync();

            Assert.Equal(StatusCodes.Status503ServiceUnavailable, result.Status());
            Assert.Equal("down", result.Body<List<ReleaseDto>>().Message);
        }

        [Fact]
        public async Task GetAllAsync_Releases_ReturnsList()
        {
            _releaseService.Setup(s => s.GetReleasesAsync()).ReturnsAsync([new ReleaseDto { TagName = "v1.0.0" }]);
            var sut = new ReleasesController(_releaseService.Object).WithUser(Guid.NewGuid());

            var result = await sut.GetAllAsync();

            Assert.Equal("v1.0.0", Assert.Single(result.Body<List<ReleaseDto>>().Value!).TagName);
        }

        [Fact]
        public async Task GetLatestAsync_NoReleases_ReturnsOkWithoutValue()
        {
            _releaseService.Setup(s => s.GetLatestReleaseAsync()).ReturnsAsync((ReleaseDto?)null);
            var sut = new ReleasesController(_releaseService.Object).WithUser(null);

            var result = await sut.GetLatestAsync();

            Assert.Equal(StatusCodes.Status200OK, result.Status());
            Assert.Null(result.Body<ReleaseDto>().Value);
        }

        [Fact]
        public void GetVersion_Always_ReturnsApiVersion()
        {
            var sut = new ReleasesController(_releaseService.Object).WithUser(null);

            var result = sut.GetVersion();

            Assert.Equal(AppVersion.DevVersion, result.Body<string>().Value);
        }
    }
}
