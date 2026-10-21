using StockFlow.Shared.DTOs.Statistics;

namespace StockFlow.ManagementApp.Interfaces
{
    public interface IStatisticsService
    {
        Task<(StatisticsDto?, string error)> GetOverviewAsync(DateTime from, DateTime to);
    }
}
