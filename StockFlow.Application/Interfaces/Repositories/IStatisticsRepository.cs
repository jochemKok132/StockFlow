using StockFlow.Domain.Entities.AuditLogs;

namespace StockFlow.Application.Interfaces.Repositories
{
    public interface IStatisticsRepository
    {
        Task<List<CashRegisterLogs>> GetPurchasesAsync(DateTime fromUtc, DateTime toUtc);
        Task<int> CountNewCustomersAsync(DateTime fromUtc, DateTime toUtc);
        Task<List<(DateTime CreatedAt, int Amount, int ReturnedAmount, decimal Price)>> GetProductLinesAsync(Guid productId, DateTime fromUtc, DateTime toUtc);
    }
}
