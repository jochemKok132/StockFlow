using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.ManagementApp.Interfaces.AuditLogs
{
    public interface ICashRegisterLogsService
    {
        Task<(List<CashRegisterLogsDto>, string error)> GetPaginatedCashRegisterLogsAsync(CashRegisterLogsPaginationDto paginationDto);
    }
}
