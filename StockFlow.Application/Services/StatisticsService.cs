using StockFlow.Application.Interfaces;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Statistics;

namespace StockFlow.Application.Services
{
    public class StatisticsService(IStatisticsRepository statisticsRepository) : IStatisticsService
    {
        public const int DefaultDays = 30;
        public const int MaxDays = 366;
        public const int TopProductCount = 10;

        public async Task<StatisticsDto> GetOverviewAsync(DateTime? from, DateTime? to)
        {
            DateTime lastDay = (to ?? DateTime.Today).Date;
            DateTime firstDay = (from ?? lastDay.AddDays(-(DefaultDays - 1))).Date;

            if (lastDay < firstDay)
                throw new ArgumentException("The end date can't be before the start date.");

            if ((lastDay - firstDay).TotalDays >= MaxDays)
                throw new ArgumentException($"Choose a period of at most {MaxDays} days.");

            DateTime fromUtc = DateTime.SpecifyKind(firstDay, DateTimeKind.Local).ToUniversalTime();
            DateTime toUtc = DateTime.SpecifyKind(lastDay.AddDays(1), DateTimeKind.Local).ToUniversalTime();

            var purchases = await statisticsRepository.GetPurchasesAsync(fromUtc, toUtc);
            int newCustomers = await statisticsRepository.CountNewCustomersAsync(fromUtc, toUtc);

            var lines = purchases.SelectMany(p => p.ProductsSold).ToList();
            decimal revenue = purchases.Sum(p => (decimal)p.TotalPrice);
            int withCustomer = purchases.Count(p => p.CustomerId is not null);

            var byDay = purchases
                .GroupBy(p => DateTime.SpecifyKind(p.CreatedAt, DateTimeKind.Utc).ToLocalTime().Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            var days = new List<DailyStatisticsDto>();
            for (DateTime day = firstDay; day <= lastDay; day = day.AddDays(1))
            {
                var dayPurchases = byDay.GetValueOrDefault(day) ?? [];
                days.Add(new DailyStatisticsDto
                {
                    Date = day,
                    Purchases = dayPurchases.Count,
                    Revenue = dayPurchases.Sum(p => (decimal)p.TotalPrice),
                    CustomerCardPercentage = Percentage(dayPurchases.Count(p => p.CustomerId is not null), dayPurchases.Count),
                });
            }

            var topProducts = lines
                .Where(l => l.Product is not null)
                .GroupBy(l => l.Product!.Id)
                .Select(g => new TopProductDto
                {
                    Barcode = g.First().Product!.Barcode,
                    ProductName = g.First().Product!.ProductName,
                    ItemsSold = g.Sum(l => l.Amount - l.ReturnedAmount),
                    Revenue = g.Sum(l => SalePricing.Total(l.PriceAtTimeOfSale, l.Amount - l.ReturnedAmount)),
                })
                .Where(p => p.ItemsSold > 0)
                .OrderByDescending(p => p.ItemsSold)
                .ThenByDescending(p => p.Revenue)
                .Take(TopProductCount)
                .ToList();

            return new StatisticsDto
            {
                From = firstDay,
                To = lastDay,
                Purchases = purchases.Count,
                Revenue = revenue,
                AverageReceipt = purchases.Count == 0 ? 0 : Math.Round(revenue / purchases.Count, 2),
                PurchasesWithCustomer = withCustomer,
                CustomerCardPercentage = Percentage(withCustomer, purchases.Count),
                ItemsSold = lines.Sum(l => l.Amount),
                ItemsReturned = lines.Sum(l => l.ReturnedAmount),
                Discounts = purchases.Sum(p => (decimal)p.TotalOff),
                PointsUsed = purchases.Sum(p => p.PointsUsed),
                PointsEarned = purchases.Sum(p => p.PointsEarned),
                NewCustomers = newCustomers,
                Days = days,
                TopProducts = topProducts,
            };
        }

        private static double Percentage(int part, int total) =>
            total == 0 ? 0 : Math.Round(part * 100.0 / total, 1);
    }
}
