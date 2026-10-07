using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.DTOs.CashRegister;
using StockFlow.Application.DTOs.Stock.ProductBrand;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Interfaces.CashRegister;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Services.ManagementApp.Stock;
using System.Security.Claims;

namespace StockFlow.API.Controllers.CashRegister
{
    [Authorize(Roles = "Cashier,ServiceDesk,ShiftLeader,Manager,Admin")]
    [ApiController]
    [Route("[controller]")]
    public class CashRegisterController(IRegisterDataService registerDataService, IProductService productService) : ControllerBase
    {
        [HttpPost("Purchase")]
        public async Task<IActionResult> FinalizePurchaseAsync(Purchase purchase)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await registerDataService.FinalizePurchase(purchase, Guid.Parse(userId));
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

        [HttpGet("{input}")]
        public async Task<IActionResult> GetItemAsync(string input)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                if (input.Length == 9)
                {
                    if (Int32.TryParse(input, out int parsed))
                        return Ok(await productService.GetProductByBarcodeAsync(parsed));

                    return BadRequest("Input must be numeric.");
                }
                else
                    return NoContent();
            }
            catch(KeyNotFoundException exception)
            {
                return BadRequest(exception.Message);
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
    }
}
