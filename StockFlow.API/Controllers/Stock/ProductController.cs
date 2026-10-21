using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Application.Interfaces.ManagementApp.Stock;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProductPaginationDto pagination)
        {
            try
            {
                List<ProductBulkViewDto> items = await productService.GetAllProductsAsync(pagination);
                return Ok(ApiResponse<List<ProductBulkViewDto>>.Ok(items));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<ProductBulkViewDto>>.Fail(exception.Message));
            }
        }

        [Authorize]
        [HttpGet("GetById/{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                ProductDto product = await productService.GetProductDetailsAsync(id);
                return Ok(ApiResponse<ProductDto>.Ok(product));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<ProductDto>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.ManageStock)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(CreateProductDto request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await productService.CreateProductAsync(request, userId);
                return Ok(ApiResponse<object>.Ok(message: "Product created."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<object>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.ManageStock)]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync(UpdateProductDto request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await productService.UpdateProductAsync(request, userId);
                return Ok(ApiResponse<object>.Ok(message: "Product updated."));
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
                await productService.SoftDeleteProductAsync(id, userId);
                return Ok(ApiResponse<object>.Ok(message: "Product deleted."));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<object>.Fail(exception.Message));
            }
        }
    }
}
