using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Shelf;
using StockFlow.Application.Interfaces.ManagementApp.Stock;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShelfController(IShelfService shelfService) : ControllerBase
    {
        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] ShelfPaginationDto pagination)
        {
            try
            {
                List<ShelfDto> items = await shelfService.GetAllShelvesAsync(pagination);
                return Ok(ApiResponse<List<ShelfDto>>.Ok(items));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<ShelfDto>>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(CreateShelfDto request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await shelfService.CreateShelfAsync(request, userId);
                return Ok(ApiResponse<object>.Ok(message: "Shelf created."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<object>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync(UpdateShelfDto request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await shelfService.UpdateShelfAsync(request, userId);
                return Ok(ApiResponse<object>.Ok(message: "Shelf updated."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<object>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<object>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpDelete("SoftDelete/{id:guid}")]
        public async Task<IActionResult> SoftDeleteAsync(Guid id)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await shelfService.SoftDeleteShelfAsync(id, userId);
                return Ok(ApiResponse<object>.Ok(message: "Shelf deleted."));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<object>.Fail(exception.Message));
            }
        }
    }
}
