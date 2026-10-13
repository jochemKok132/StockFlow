using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Sales;
using StockFlow.Application.Interfaces.ManagementApp.Stock;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController(ISalesService salesService) : ControllerBase
    {
        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] SalesPaginationDto pagination)
        {
            try
            {
                List<SalesDto> items = await salesService.GetAllSalesAsync(pagination);
                return Ok(ApiResponse<List<SalesDto>>.Ok(items));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<SalesDto>>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(CreateSalesDto request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await salesService.CreateSalesAsync(request, userId);
                return Ok(ApiResponse<object>.Ok(message: "Sale created."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<object>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync(UpdateSalesDto request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await salesService.UpdateSalesAsync(request, userId);
                return Ok(ApiResponse<object>.Ok(message: "Sale updated."));
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
                await salesService.SoftDeleteSalesAsync(id, userId);
                return Ok(ApiResponse<object>.Ok(message: "Sale deleted."));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<object>.Fail(exception.Message));
            }
        }
    }
}
