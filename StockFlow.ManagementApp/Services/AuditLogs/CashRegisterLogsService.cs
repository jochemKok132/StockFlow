using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.AuditLogs;

namespace StockFlow.ManagementApp.Services.AuditLogs
{
    public class CashRegisterLogsService(IHttpService httpService) : ICashRegisterLogsService
    {
        public async Task<(byte[]?, string error)> GetReceiptPdfAsync(string receiptId)
        {
            var response = await httpService.GetFileAsync($"CashRegisterLogs/Receipt/{Uri.EscapeDataString(receiptId)}/Pdf");
            return response.Succeeded
                ? (response.Value, "")
                : (null, $"Error: {response.Message}");
        }

        public async Task<(List<CashRegisterLogsDto>, string error)> GetPaginatedCashRegisterLogsAsync(CashRegisterLogsPaginationDto pagination)
        {
            var url = "CashRegisterLogs/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&receiptId={pagination.ReceiptId}" +
                $"&employeeName={pagination.EmployeeName}" +
                $"&employeeId={pagination.EmployeeId}" +
                $"&email={pagination.Email}" +
                $"&customerName={pagination.CustomerName}";
                
            var response = await httpService.GetAsync<List<CashRegisterLogsDto>>(url);
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new List<CashRegisterLogsDto>(), $"Error: {response.Message}");
        }
    }
}