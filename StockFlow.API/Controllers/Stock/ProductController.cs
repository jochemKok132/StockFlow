using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateProductAsync(CreateProductDto product)
        {
            try
            {
                await productService.CreateProductAsync(product);
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
            try
            {
                await productService.UpdateProductAsync(product);
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
        public async Task<IActionResult> SoftDeleteProductAsync(Guid id)
        {
            try
            {
                await productService.SoftDeleteProductAsync(id);
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