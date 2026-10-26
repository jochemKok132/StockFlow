using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Shared.DTOs.AuditLogs.CashRegister;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CashRegisterLogsController(ICashRegisterLogsService cashRegisterLogsService, IReceiptPdfService receiptPdfService) : ControllerBase
    {
        [Authorize(Roles = Permissions.CashRegisterLogs)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] CashRegisterLogsPaginationDto pagination)
        {
            try
            {
                List<CashRegisterLogsDto> logs = await cashRegisterLogsService.GetAllCashRegisterLogsAsync(pagination);
                return Ok(ApiResponse<List<CashRegisterLogsDto>>.Ok(logs));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<CashRegisterLogsDto>>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.CashRegisterLogs)]
        [HttpGet("Receipt/{receiptId}/Pdf")]
        public async Task<IActionResult> GetReceiptPdfAsync(string receiptId)
        {
            try
            {
                CashRegisterLogsDto receipt = await cashRegisterLogsService.GetByReceiptIdAsync(receiptId);
                return File(receiptPdfService.Create(receipt), "application/pdf", $"receipt-{receipt.ReceiptId}.pdf");
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

        [Authorize(Roles = Permissions.WriteLogs)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(CreateCashRegisterLogsDto log)
        {
            try
            {
                string receiptId = await cashRegisterLogsService.CreateCashRegisterLogsAsync(log);
                return Ok(ApiResponse<string>.Ok(receiptId, "Log created."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<string>.Fail(exception.Message));
            }
        }
    }
}
