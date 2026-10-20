using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Shared.DTOs.AuditLogs.Customer;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerLogsController(ICustomerLogsService customerLogsService) : ControllerBase
    {
        [Authorize(Roles = "Manager,Admin,Tester")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] CustomerLogsPaginationDto pagination)
        {
            try
            {
                List<CustomerLogsDto> logs = await customerLogsService.GetAllCustomerLogsAsync(pagination);
                return Ok(ApiResponse<List<CustomerLogsDto>>.Ok(logs));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<CustomerLogsDto>>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(CreateCustomerLogsDto log)
        {
            try
            {
                await customerLogsService.CreateCustomerLogsAsync(log);
                return Ok(ApiResponse<object>.Ok(message: "Log created."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<object>.Fail(exception.Message));
            }
        }
    }
}
