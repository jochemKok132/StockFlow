using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;

namespace StockFlow.ManagementApp.Interfaces.AuditLogs
{
    public interface ICustomerLogsService
    {
        Task<(List<CustomerLogsDto>, string error)> GetPaginatedCustomerLogsAsync(CustomerLogsPaginationDto paginationDto);
    }
}
