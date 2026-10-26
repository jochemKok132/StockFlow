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
    public class ProductController(IProductService productService, IProductControlService productControlService) : ControllerBase
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

        [Authorize(Roles = Permissions.WriteOffStock)]
        [HttpPost("WriteOff")]
        public async Task<IActionResult> WriteOffAsync(WriteOffRequest request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<int>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                int stock = await productService.WriteOffStockAsync(request, userId);
                return Ok(ApiResponse<int>.Ok(stock, $"{request.Amount} written off. Stock is now {stock}."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<int>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<int>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.ManageStock)]
        [HttpPost("Price")]
        public Task<IActionResult> SchedulePriceAsync(SchedulePriceRequest request) =>
            this.RunAsEmployeeAsync(userId => productControlService.SchedulePriceAsync(request, userId),
                _ => request.ValidFrom.Date <= DateTime.Today ? "Price changed." : $"New price planned from {request.ValidFrom:dd-MM-yyyy}.");

        [Authorize(Roles = Permissions.ManageStock)]
        [HttpDelete("{productId:guid}/Price/{priceId:guid}")]
        public Task<IActionResult> CancelPriceAsync(Guid productId, Guid priceId) =>
            this.RunAsEmployeeAsync(userId => productControlService.CancelScheduledPriceAsync(productId, priceId, userId), _ => "Planned price cancelled.");

        [Authorize(Roles = Permissions.ManageStock)]
        [HttpPost("Recall")]
        public Task<IActionResult> RecallAsync(RecallRequest request) =>
            this.RunAsEmployeeAsync(userId => productControlService.RecallAsync(request, userId), p => $"{p.ProductName} is recalled.");

        [Authorize(Roles = Permissions.ManageStock)]
        [HttpDelete("{productId:guid}/Recall")]
        public Task<IActionResult> UndoRecallAsync(Guid productId) =>
            this.RunAsEmployeeAsync(userId => productControlService.UndoRecallAsync(productId, userId), p => $"The recall of {p.ProductName} is lifted.");

        [Authorize(Roles = Permissions.ManageStock)]
        [HttpPost("OrderBlock")]
        public Task<IActionResult> BlockOrdersAsync(OrderBlockRequest request) =>
            this.RunAsEmployeeAsync(userId => productControlService.BlockOrdersAsync(request, userId), p => $"{p.ProductName} is blocked for ordering.");

        [Authorize(Roles = Permissions.ManageStock)]
        [HttpDelete("{productId:guid}/OrderBlock")]
        public Task<IActionResult> UnblockOrdersAsync(Guid productId) =>
            this.RunAsEmployeeAsync(userId => productControlService.UnblockOrdersAsync(productId, userId), p => $"{p.ProductName} can be ordered again.");

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
