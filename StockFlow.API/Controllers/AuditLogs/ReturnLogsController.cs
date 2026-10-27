using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.AuditLogs.Return;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReturnLogsController(IReturnLogsService returnLogsService) : ControllerBase
    {
        [Authorize(Roles = Permissions.CashRegisterLogs)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] ReturnLogsPaginationDto pagination)
        {
            try
            {
                List<ReturnLogDto> logs = await returnLogsService.GetAllReturnLogsAsync(pagination);
                return Ok(ApiResponse<List<ReturnLogDto>>.Ok(logs));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<ReturnLogDto>>.Fail(exception.Message));
            }
        }
    }
}
