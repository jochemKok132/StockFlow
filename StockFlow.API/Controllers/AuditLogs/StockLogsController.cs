using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Shared.DTOs.AuditLogs.Stock;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockLogsController(IStockLogsService stockLogsService) : ControllerBase
    {
        [Authorize(Roles = Permissions.StockLogs)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] StockLogsPaginationDto pagination)
        {
            try
            {
                List<StockLogsDto> logs = await stockLogsService.GetAllStockLogsAsync(pagination);
                return Ok(ApiResponse<List<StockLogsDto>>.Ok(logs));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<StockLogsDto>>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.StockLogs)]
        [HttpGet("ProductHistory/{barcode:int}")]
        public async Task<IActionResult> GetProductHistoryAsync(int barcode, [FromQuery] ProductHistoryPaginationDto pagination)
        {
            try
            {
                ProductStockHistoryDto history = await stockLogsService.GetProductStockHistoryAsync(barcode, pagination);
                return Ok(ApiResponse<ProductStockHistoryDto>.Ok(history));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<ProductStockHistoryDto>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<ProductStockHistoryDto>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.WriteLogs)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(CreateStockLogsDto log)
        {
            try
            {
                await stockLogsService.CreateStockLogsAsync(log);
                return Ok(ApiResponse<object>.Ok(message: "Log created."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<object>.Fail(exception.Message));
            }
        }
    }
}
