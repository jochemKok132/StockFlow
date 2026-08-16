using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Shelf;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShelfController(IShelfService shelfService) : ControllerBase
    {
        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllShelvesAsync([FromQuery] ShelfPaginationDto pagination)
        {
            try
            {
                var shelves = await shelfService.GetAllShelvesAsync(pagination);
                return Ok(shelves);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateShelfAsync(CreateShelfDto shelf)
        {
            try
            {
                await shelfService.CreateShelfAsync(shelf);
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
        public async Task<IActionResult> UpdateShelfAsync(UpdateShelfDto shelf)
        {
            try
            {
                await shelfService.UpdateShelfAsync(shelf);
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
        public async Task<IActionResult> SoftDeleteShelfAsync(Guid id)
        {
            try
            {
                await shelfService.SoftDeleteShelfAsync(id);
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