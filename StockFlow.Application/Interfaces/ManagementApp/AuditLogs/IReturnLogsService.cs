using StockFlow.Shared.DTOs.AuditLogs.Return;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.Application.Interfaces.ManagementApp.AuditLogs
{
    public interface IReturnLogsService
    {
        Task<List<ReturnLogDto>> GetAllReturnLogsAsync(ReturnLogsPaginationDto pagination);
    }
}
