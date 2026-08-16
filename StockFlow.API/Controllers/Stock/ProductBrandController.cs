using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.ProductBrand;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                await productBrandService.CreateProductBrandAsync(productBrand);
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
            try
            {
                await productBrandService.UpdateProductBrandAsync(productBrand);
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
        public async Task<IActionResult> SoftDeleteProductBrandAsync(Guid id)
        {
            try
            {
                await productBrandService.SoftDeleteProductBrandAsync(id);
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