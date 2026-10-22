using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Interfaces;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeletedItemsController(IDeletedItemsService deletedItemsService) : ControllerBase
    {
        [Authorize(Roles = Permissions.ViewDeleted)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] DeletedItemTypeDto? type)
        {
            try
            {
                return Ok(ApiResponse<List<DeletedItemDto>>.Ok(await deletedItemsService.GetDeletedItemsAsync(type)));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<DeletedItemDto>>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.RestoreDeleted)]
        [HttpPost("Restore/{type}/{id:guid}")]
        public async Task<IActionResult> RestoreAsync(DeletedItemTypeDto type, Guid id)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<DeletedItemDto>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                DeletedItemDto item = await deletedItemsService.RestoreAsync(type, id, userId);
                return Ok(ApiResponse<DeletedItemDto>.Ok(item, $"{item.Name} is restored."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<DeletedItemDto>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<DeletedItemDto>.Fail(exception.Message));
            }
        }
    }
}
