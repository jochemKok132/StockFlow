using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.AuditLogs;
using StockFlow.Shared.DTOs.AuditLogs.Return;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.ManagementApp.Services.AuditLogs
{
    public class ReturnLogsService(IHttpService httpService) : IReturnLogsService
    {
        public async Task<(List<ReturnLogDto>, string error)> GetPaginatedReturnLogsAsync(ReturnLogsPaginationDto pagination)
        {
            var url = "ReturnLogs/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&receiptId={Uri.EscapeDataString(pagination.ReceiptId ?? "")}" +
                $"&employeeId={Uri.EscapeDataString(pagination.EmployeeId ?? "")}" +
                $"&customerId={Uri.EscapeDataString(pagination.CustomerId ?? "")}";

            var response = await httpService.GetAsync<List<ReturnLogDto>>(url);
            return response.Succeeded
                ? (response.Value!, "")
                : ([], $"Error: {response.Message}");
        }
    }
}
