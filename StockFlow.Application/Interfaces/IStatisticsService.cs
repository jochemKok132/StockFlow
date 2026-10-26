using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Statistics;

namespace StockFlow.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<StatisticsDto> GetOverviewAsync(DateTime? from, DateTime? to);
        Task<ProductSalesDto> GetProductSalesAsync(Guid productId, DateTime? from, DateTime? to);
    }
}
