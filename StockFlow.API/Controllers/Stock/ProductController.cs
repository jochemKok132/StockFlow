using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllProductsAsync([FromQuery] ProductPaginationDto pagination)
        {
            try
            {
                var products = await productService.GetAllProductsAsync(pagination);
                return Ok(products);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }
        [Authorize]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetProductByIdAsync( Guid id)
        {
            try
            {
                var products = await productService.GetProductDetailsAsync(id);
                return Ok(products);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateProductAsync(CreateProductDto product)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await productService.CreateProductAsync(product, Guid.Parse(userId));
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
        public async Task<IActionResult> UpdateProductAsync(UpdateProductDto product)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await productService.UpdateProductAsync(product, Guid.Parse(userId));
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
        public async Task<IActionResult> SoftDeleteProductAsync(Guid id)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await productService.SoftDeleteProductAsync(id, Guid.Parse(userId));
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