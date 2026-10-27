using StockFlow.Shared.DTOs.AuditLogs.Return;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.ManagementApp.Interfaces.AuditLogs
{
    public interface IReturnLogsService
    {
        Task<(List<ReturnLogDto>, string error)> GetPaginatedReturnLogsAsync(ReturnLogsPaginationDto pagination);
    }
}
