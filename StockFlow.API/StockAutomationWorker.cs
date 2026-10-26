using StockFlow.Application.Interfaces.Ordering;

namespace StockFlow.API
{
    public class StockAutomationWorker(IServiceScopeFactory scopeFactory, ILogger<StockAutomationWorker> logger) : BackgroundService
    {
        public static readonly TimeSpan Interval = TimeSpan.FromMinutes(15);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);
            do
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var automation = scope.ServiceProvider.GetRequiredService<IStockAutomationService>();
                    var result = await automation.RunAsync(DateTime.Today);
                    if (result.PricesApplied + result.ExpiryCountsCreated + result.AutomaticOrderLines + result.OrdersSent > 0)
                        logger.LogInformation("Stock automation: {Prices} price(s) applied, {Counts} expiry count(s) created, {Lines} automatic order line(s), {Sent} order(s) sent.",
                            result.PricesApplied, result.ExpiryCountsCreated, result.AutomaticOrderLines, result.OrdersSent);
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    logger.LogError(exception, "Stock automation failed.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
