using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Repositories
{
    public class StatisticsRepository(ApplicationDBContext context) : IStatisticsRepository
    {
        public Task<List<CashRegisterLogs>> GetPurchasesAsync(DateTime fromUtc, DateTime toUtc) =>
            context.CashRegisterLogs
                .AsNoTracking()
                .Include(l => l.ProductsSold)
                    .ThenInclude(d => d.Product)
                .Where(l => l.CreatedAt >= fromUtc && l.CreatedAt < toUtc)
                .ToListAsync();

        public async Task<List<(DateTime CreatedAt, int Amount, int ReturnedAmount, decimal Price)>> GetProductLinesAsync(Guid productId, DateTime fromUtc, DateTime toUtc)
        {
            var rows = await context.RegisterItemDetails
                .AsNoTracking()
                .Where(d => d.ProductId == productId)
                .Join(context.CashRegisterLogs, d => d.CashregisterLogId, l => l.Id,
                    (d, l) => new { l.CreatedAt, d.Amount, d.ReturnedAmount, d.PriceAtTimeOfSale })
                .Where(x => x.CreatedAt >= fromUtc && x.CreatedAt < toUtc)
                .ToListAsync();
            return rows.Select(r => (r.CreatedAt, r.Amount, r.ReturnedAmount, r.PriceAtTimeOfSale)).ToList();
        }

        public Task<int> CountNewCustomersAsync(DateTime fromUtc, DateTime toUtc) =>
            context.Customers.CountAsync(c => !c.SoftDeleted && c.CreatedAt >= fromUtc && c.CreatedAt < toUtc);
    }
}
