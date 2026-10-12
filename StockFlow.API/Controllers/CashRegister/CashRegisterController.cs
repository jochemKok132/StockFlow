using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Common;
using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.CashRegister;
using StockFlow.Application.DTOs.Customer;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Interfaces.CashRegister;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;

namespace StockFlow.API.Controllers.CashRegister
{
    [Authorize(Roles = "Cashier,ServiceDesk,ShiftLeader,Manager,Admin")]
    [ApiController]
    [Route("[controller]")]
    public class CashRegisterController(IRegisterDataService registerDataService, IProductService productService, ICustomerService customerService, ICashRegisterLogsService cashRegisterLogsService) : ControllerBase
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
            if (!IdentifierFormats.IsCustomerId(customerId))
                return BadRequest(ApiResponse<CustomerDto>.Fail(
                    $"A customer id must be {IdentifierFormats.CustomerIdLength} digits and start with {IdentifierFormats.CustomerIdPrefix}."));

            try
            {
                CustomerDto customer = await customerService.GetCustomerByCustomerIdAsync(customerId);
                return Ok(ApiResponse<CustomerDto>.Ok(customer));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<CustomerDto>.Fail(exception.Message));
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
