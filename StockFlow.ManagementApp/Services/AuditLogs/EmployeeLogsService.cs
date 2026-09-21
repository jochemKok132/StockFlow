using StockFlow.Application.DTOs.AuditLogs.Employee;
using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.AuditLogs;

namespace StockFlow.ManagementApp.Services.AuditLogs
{
    public class EmployeeLogsService(IHttpService httpService) : IEmployeeLogsService
    {
        public async Task<(List<EmployeeLogsDto>, string error)> GetPaginatedEmployeeLogsAsync(EmployeeLogsPaginationDto pagination)
        {
            var url = "EmployeeLogs/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&employeeName={pagination.EmployeeName}" +
                $"&identifier={pagination.Identifier}" +
                $"&employeeId={pagination.EmployeeId}" +
                $"&logType={pagination.LogType}";
                
            var response = await httpService.GetAsync<List<EmployeeLogsDto>>(url);
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new List<EmployeeLogsDto>(), $"Error: {response.Message}");
        }
    }
}