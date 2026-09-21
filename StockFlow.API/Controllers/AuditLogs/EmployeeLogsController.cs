using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.DTOs.AuditLogs.Employee;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeLogsController(IEmployeeLogsService EmployeeLogsService) : ControllerBase
    {
        [Authorize(Roles = "ServiceDesk,ShiftLeader,Manager,Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllEmployeeLogsAsync([FromQuery] EmployeeLogsPaginationDto pagination)
        {
            try
            {
                var EmployeeLogs = await EmployeeLogsService.GetAllEmployeeLogsAsync(pagination);
                return Ok(EmployeeLogs);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateEmployeeLogsAsync(CreateEmployeeLogsDto log)
        {
            try
            {
                await EmployeeLogsService.CreateEmployeeLogsAsync(log);
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