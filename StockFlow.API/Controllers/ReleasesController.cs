using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Interfaces;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Releases;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReleasesController(IReleaseService releaseService) : ControllerBase
    {
        [Authorize(Roles = Permissions.Employees)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                List<ReleaseDto> releases = await releaseService.GetReleasesAsync();
                return Ok(ApiResponse<List<ReleaseDto>>.Ok(releases));
            }
            catch (HttpRequestException exception)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiResponse<List<ReleaseDto>>.Fail(exception.Message));
            }
        }

        [AllowAnonymous]
        [HttpGet("Latest")]
        public async Task<IActionResult> GetLatestAsync()
        {
            try
            {
                ReleaseDto? release = await releaseService.GetLatestReleaseAsync();
                return Ok(ApiResponse<ReleaseDto>.Ok(release, release is null ? "There are no releases yet." : null));
            }
            catch (HttpRequestException exception)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiResponse<ReleaseDto>.Fail(exception.Message));
            }
        }

        [AllowAnonymous]
        [HttpGet("Version")]
        public IActionResult GetVersion() =>
            Ok(ApiResponse<string>.Ok(AppVersion.Of(typeof(ReleasesController).Assembly)));
    }
}
