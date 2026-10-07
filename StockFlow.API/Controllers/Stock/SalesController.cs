using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Sales;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController(ISalesService salesService) : ControllerBase
    {
        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllSalesAsync([FromQuery] SalesPaginationDto pagination)
        {
            try
            {
                var sales = await salesService.GetAllSalesAsync(pagination);
                return Ok(sales);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateSalesAsync(CreateSalesDto sale)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await salesService.CreateSalesAsync(sale, Guid.Parse(userId));
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

        [Authorize(Roles = "Manager,Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateSalesAsync(UpdateSalesDto sale)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await salesService.UpdateSalesAsync(sale, Guid.Parse(userId));
                return NoContent();
            }
            catch (KeyNotFoundException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpDelete("SoftDelete/{id}")]
        public async Task<IActionResult> SoftDeleteSalesAsync(Guid id)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await salesService.SoftDeleteSalesAsync(id, Guid.Parse(userId));
                return NoContent();
            }
            catch (KeyNotFoundException exception)
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