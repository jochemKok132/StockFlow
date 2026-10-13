using StockFlow.Shared.DTOs.AuditLogs.Employee;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.ManagementApp.Interfaces.AuditLogs
{
    public interface IEmployeeLogsService
    {
        Task<(List<EmployeeLogsDto>, string error)> GetPaginatedEmployeeLogsAsync(EmployeeLogsPaginationDto paginationDto);
    }
}
