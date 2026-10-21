using StockFlow.ManagementApp.Interfaces;
using StockFlow.Shared.DTOs.Statistics;

namespace StockFlow.ManagementApp.Services
{
    public class StatisticsService(IHttpService httpService) : IStatisticsService
    {
        public async Task<(StatisticsDto?, string error)> GetOverviewAsync(DateTime from, DateTime to)
        {
            var response = await httpService.GetAsync<StatisticsDto>($"Statistics/Overview?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
            return response.Succeeded
                ? (response.Value, "")
                : (null, response.Message ?? "The statistics could not be loaded.");
        }
    }
}
