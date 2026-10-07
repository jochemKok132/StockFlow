using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.ProductBrand;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using System.Security.Claims;
using System.Security.Cryptography;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductBrandController(IProductBrandService productBrandService) : ControllerBase
    {
        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllProductBrandsAsync([FromQuery] ProductBrandPaginationDto pagination)
        {
            try
            {
                var productBrands = await productBrandService.GetAllProductBrandsAsync(pagination);
                return Ok(productBrands);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateProductBrandAsync(CreateProductBrandDto productBrand)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await productBrandService.CreateProductBrandAsync(productBrand, Guid.Parse(userId));
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
        public async Task<IActionResult> UpdateProductBrandAsync(UpdateProductBrandDto productBrand)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await productBrandService.UpdateProductBrandAsync(productBrand, Guid.Parse(userId));
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
        public async Task<IActionResult> SoftDeleteProductBrandAsync(Guid id)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await productBrandService.SoftDeleteProductBrandAsync(id, Guid.Parse(userId));
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