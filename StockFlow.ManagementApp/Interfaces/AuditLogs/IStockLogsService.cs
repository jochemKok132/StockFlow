using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;

namespace StockFlow.ManagementApp.Interfaces.AuditLogs
{
    public interface IStockLogsService
    {
        Task<(List<StockLogsDto>, string error)> GetPaginatedStockLogsAsync(StockLogsPaginationDto paginationDto);
    }
}
