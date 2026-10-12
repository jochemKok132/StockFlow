using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Common;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.DTOs.AuditLogs.Employee;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeLogsController(IEmployeeLogsService employeeLogsService) : ControllerBase
    {
        [Authorize(Roles = "ServiceDesk,ShiftLeader,Manager,Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] EmployeeLogsPaginationDto pagination)
        {
            try
            {
                List<EmployeeLogsDto> logs = await employeeLogsService.GetAllEmployeeLogsAsync(pagination);
                return Ok(ApiResponse<List<EmployeeLogsDto>>.Ok(logs));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<EmployeeLogsDto>>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(CreateEmployeeLogsDto log)
        {
            try
            {
                await employeeLogsService.CreateEmployeeLogsAsync(log);
                return Ok(ApiResponse<object>.Ok(message: "Log created."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<object>.Fail(exception.Message));
            }
        }
    }
}
