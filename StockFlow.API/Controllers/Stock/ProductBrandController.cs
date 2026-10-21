using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.ProductBrand;
using StockFlow.Application.Interfaces.ManagementApp.Stock;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductBrandController(IProductBrandService productBrandService) : ControllerBase
    {
        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductBrandPaginationDto pagination)
        {
            try
            {
                List<ProductBrandDto> items = await productBrandService.GetAllProductBrandsAsync(pagination);
                return Ok(ApiResponse<List<ProductBrandDto>>.Ok(items));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<ProductBrandDto>>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.ManageStock)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(CreateProductBrandDto request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await productBrandService.CreateProductBrandAsync(request, userId);
                return Ok(ApiResponse<object>.Ok(message: "Brand created."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<object>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.ManageStock)]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync(UpdateProductBrandDto request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await productBrandService.UpdateProductBrandAsync(request, userId);
                return Ok(ApiResponse<object>.Ok(message: "Brand updated."));
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

        [Authorize(Roles = Permissions.ManageStock)]
        [HttpDelete("SoftDelete/{id:guid}")]
        public async Task<IActionResult> SoftDeleteAsync(Guid id)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await productBrandService.SoftDeleteProductBrandAsync(id, userId);
                return Ok(ApiResponse<object>.Ok(message: "Brand deleted."));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<object>.Fail(exception.Message));
            }
        }
    }
}
