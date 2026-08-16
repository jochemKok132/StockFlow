using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockLogsController(IStockLogsService stockLogsService) : ControllerBase
    {
        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllStockLogsAsync([FromQuery] StockLogsPaginationDto pagination)
        {
            try
            {
                var stockLogs = await stockLogsService.GetAllStockLogsAsync(pagination);
                return Ok(stockLogs);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateStockLogsAsync(CreateStockLogsDto log)
        {
            try
            {
                await stockLogsService.CreateStockLogsAsync(log);
                return NoContent();
            }
            catch (ArgumentNullException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }
    }
}