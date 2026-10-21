using StockFlow.Domain.Entities.AuditLogs;

namespace StockFlow.Application.Interfaces.Repositories
{
    public interface IStatisticsRepository
    {
        Task<List<CashRegisterLogs>> GetPurchasesAsync(DateTime fromUtc, DateTime toUtc);
        Task<int> CountNewCustomersAsync(DateTime fromUtc, DateTime toUtc);
    }
}
