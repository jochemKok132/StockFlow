using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Interfaces.CashRegister;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;

namespace StockFlow.API.Controllers.CashRegister
{
    [Authorize(Roles = Permissions.CashRegister)]
    [ApiController]
    [Route("[controller]")]
    public class CashRegisterController(IRegisterDataService registerDataService, IProductService productService, ICashRegisterLogsService cashRegisterLogsService) : ControllerBase
    {
        [HttpPost("Purchase")]
        public async Task<IActionResult> FinalizePurchaseAsync(Purchase purchase)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                string receiptId = await registerDataService.FinalizePurchase(purchase, userId);
                return Ok(ApiResponse<string>.Ok(receiptId, "Purchase completed."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<string>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<string>.Fail(exception.Message));
            }
        }

        [HttpGet("Receipt/{receiptId}")]
        public async Task<IActionResult> GetReceiptAsync(string receiptId)
        {
            try
            {
                CashRegisterLogsDto receipt = await cashRegisterLogsService.GetByReceiptIdAsync(receiptId);
                return Ok(ApiResponse<CashRegisterLogsDto>.Ok(receipt));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<CashRegisterLogsDto>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<CashRegisterLogsDto>.Fail(exception.Message));
            }
        }

        [HttpPost("Return")]
        public async Task<IActionResult> ReturnAsync(ReturnRequest request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<ReturnResult>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                ReturnResult result = await registerDataService.ProcessReturn(request, userId);
                return Ok(ApiResponse<ReturnResult>.Ok(result, "Return completed."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<ReturnResult>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<ReturnResult>.Fail(exception.Message));
            }
        }

        [HttpGet("Customer/{customerId}")]
        public async Task<IActionResult> GetCustomerAsync(string customerId)
        {
            try
            {
                CustomerDto customer = await registerDataService.GetCustomerAsync(customerId);
                return Ok(ApiResponse<CustomerDto>.Ok(customer));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<CustomerDto>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<CustomerDto>.Fail(exception.Message));
            }
        }

        [HttpGet("Customer/Search")]
        public async Task<IActionResult> FindCustomersAsync([FromQuery] string postalCode, [FromQuery] string houseNumber)
        {
            try
            {
                List<CustomerDto> customers = await registerDataService.FindCustomersByAddressAsync(postalCode, houseNumber);
                return Ok(ApiResponse<List<CustomerDto>>.Ok(customers));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<CustomerDto>>.Fail(exception.Message));
            }
        }

        [HttpGet("{input}")]
        public async Task<IActionResult> GetProductAsync(string input)
        {
            if (!IdentifierFormats.IsProductBarcode(input))
                return BadRequest(ApiResponse<ProductDto>.Fail(
                    $"A product barcode must be {IdentifierFormats.ProductBarcodeLength} digits."));

            try
            {
                ProductDto product = await productService.GetProductByBarcodeAsync(int.Parse(input));
                return Ok(ApiResponse<ProductDto>.Ok(product));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<ProductDto>.Fail(exception.Message));
            }
        }
    }
}
