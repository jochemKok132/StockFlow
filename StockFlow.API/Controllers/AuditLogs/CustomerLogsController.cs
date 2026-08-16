using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerLogsController(ICustomerLogsService customerLogsService) : ControllerBase
    {
        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCustomerLogsAsync([FromQuery] CustomerLogsPaginationDto pagination)
        {
            try
            {
                var customerLogs = await customerLogsService.GetAllCustomerLogsAsync(pagination);
                return Ok(customerLogs);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateCustomerLogsAsync(CreateCustomerLogsDto log)
        {
            try
            {
                await customerLogsService.CreateCustomerLogsAsync(log);
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