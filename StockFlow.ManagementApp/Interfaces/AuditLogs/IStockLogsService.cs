using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.ManagementApp.Interfaces.AuditLogs
{
    public interface IStockLogsService
    {
        Task<(List<StockLogsDto>, string error)> GetPaginatedStockLogsAsync(StockLogsPaginationDto paginationDto);
        Task<(ProductStockHistoryDto?, string error)> GetProductStockHistoryAsync(int barcode, ProductHistoryPaginationDto pagination);
    }
}
