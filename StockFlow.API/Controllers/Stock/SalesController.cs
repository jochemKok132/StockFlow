using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Sales;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                await salesService.CreateSalesAsync(sale);
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
            try
            {
                await salesService.UpdateSalesAsync(sale);
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
        [HttpDelete("SoftDelete")]
        public async Task<IActionResult> SoftDeleteSalesAsync(Guid id)
        {
            try
            {
                await salesService.SoftDeleteSalesAsync(id);
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