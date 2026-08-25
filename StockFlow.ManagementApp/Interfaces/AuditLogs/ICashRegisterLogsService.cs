using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;

namespace StockFlow.ManagementApp.Interfaces.AuditLogs
{
    public interface ICashRegisterLogsService
    {
        Task<(List<CashRegisterLogsDto>, string error)> GetPaginatedCashRegisterLogsAsync(CashRegisterLogsPaginationDto paginationDto);
    }
}
