using StockFlow.Shared.DTOs.Statistics;

namespace StockFlow.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<StatisticsDto> GetOverviewAsync(DateTime? from, DateTime? to);
    }
}
