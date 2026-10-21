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

        public Task<int> CountNewCustomersAsync(DateTime fromUtc, DateTime toUtc) =>
            context.Customers.CountAsync(c => !c.SoftDeleted && c.CreatedAt >= fromUtc && c.CreatedAt < toUtc);
    }
}
