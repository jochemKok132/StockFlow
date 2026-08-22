using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CashRegisterLogsController(ICashRegisterLogsService cashRegisterLogsService) : ControllerBase
    {
        [Authorize(Roles = "Cashier,ServiceDesk,ShiftLeader,Manager,Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCashRegisterLogsAsync([FromQuery] CashRegisterLogsPaginationDto pagination)
        {
            try
            {
                var cashRegisterLogs = await cashRegisterLogsService.GetAllCashRegisterLogsAsync(pagination);
                return Ok(cashRegisterLogs);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateCashRegisterLogsAsync(CreateCashRegisterLogsDto log)
        {
            try
            {
                await cashRegisterLogsService.CreateCashRegisterLogsAsync(log);
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