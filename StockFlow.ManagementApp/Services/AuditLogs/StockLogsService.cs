using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.AuditLogs;

namespace StockFlow.ManagementApp.Services.AuditLogs
{
    public class StockLogsService(IHttpService httpService) : IStockLogsService
    {
        public async Task<(List<StockLogsDto>, string error)> GetPaginatedStockLogsAsync(StockLogsPaginationDto pagination)
        {
            var url = "StockLogs/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&employeeName={pagination.EmployeeName}" +
                $"&employeeId={pagination.EmployeeId}" +
                $"&stockLogType={pagination.StockLogType}" +
                $"&logType={pagination.LogType}";
            var response = await httpService.GetAsync<List<StockLogsDto>>(url);
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new List<StockLogsDto>(), $"Error: {response.Message}");
        }
    }
}