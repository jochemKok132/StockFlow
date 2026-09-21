using StockFlow.Application.DTOs.AuditLogs.Employee;
using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;

namespace StockFlow.ManagementApp.Interfaces.AuditLogs
{
    public interface IEmployeeLogsService
    {
        Task<(List<EmployeeLogsDto>, string error)> GetPaginatedEmployeeLogsAsync(EmployeeLogsPaginationDto paginationDto);
    }
}
