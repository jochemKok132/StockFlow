using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.AuditLogs;

namespace StockFlow.ManagementApp.Services.AuditLogs
{
    public class CashRegisterLogsService(IHttpService httpService) : ICashRegisterLogsService
    {
        public async Task<(List<CashRegisterLogsDto>, string error)> GetPaginatedCashRegisterLogsAsync(CashRegisterLogsPaginationDto pagination)
        {
            var url = "CashRegisterLogs/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&employeeName={pagination.EmployeeName}" +
                $"&employeeId={pagination.EmployeeId}" +
                $"&customerName={pagination.CustomerName}";
                
            var response = await httpService.GetAsync<List<CashRegisterLogsDto>>(url);
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new List<CashRegisterLogsDto>(), $"Error: {response.Message}");
        }
    }
}