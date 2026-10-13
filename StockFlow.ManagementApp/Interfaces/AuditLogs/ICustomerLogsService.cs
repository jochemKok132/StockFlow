using StockFlow.Shared.DTOs.AuditLogs.Customer;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.ManagementApp.Interfaces.AuditLogs
{
    public interface ICustomerLogsService
    {
        Task<(List<CustomerLogsDto>, string error)> GetPaginatedCustomerLogsAsync(CustomerLogsPaginationDto paginationDto);
    }
}
