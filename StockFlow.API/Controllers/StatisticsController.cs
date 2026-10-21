using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Interfaces;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Statistics;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = Permissions.Statistics)]
    public class StatisticsController(IStatisticsService statisticsService) : ControllerBase
    {
        [HttpGet("Overview")]
        public async Task<IActionResult> GetOverviewAsync([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                StatisticsDto statistics = await statisticsService.GetOverviewAsync(from, to);
                return Ok(ApiResponse<StatisticsDto>.Ok(statistics));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<StatisticsDto>.Fail(exception.Message));
            }
        }
    }
}
