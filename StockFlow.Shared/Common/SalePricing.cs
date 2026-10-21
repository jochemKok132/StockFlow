using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Stock.Sales;

namespace StockFlow.Shared.Common
{
    public static class SalePricing
    {
        public static readonly Guid MovingSaleId = new("6d0f5a1e-0000-4000-8000-000000000001");
        public const string MovingSaleName = "Moving sale";
        public const int MovingSalePercentage = 10;
        public const int MovingSaleDays = 90;
        public const int SecondItemPercentageOff = 50;

        public static bool IsInPeriod(DateTime? start, DateTime? end, DateTime today) =>
            (start is null || start.Value.Date <= today.Date) &&
            (end is null || today.Date <= end.Value.Date);

        public static SaleStatusDto Status(DateTime? start, DateTime? end, DateTime today)
        {
            if (start is not null && today.Date < start.Value.Date) return SaleStatusDto.Scheduled;
            if (end is not null && today.Date > end.Value.Date) return SaleStatusDto.Ended;
            return SaleStatusDto.Active;
        }

        public static bool IsActive(SalesDto sale, DateTime today) =>
            !sale.SoftDeleted && IsInPeriod(sale.StartDate, sale.EndDate, today);

        public static bool IsMovingSaleActive(DateTime? movingSaleUntil, DateTime today) =>
            movingSaleUntil is not null && today.Date <= movingSaleUntil.Value.Date;

        public static int FreeItems(int amount, int buy, int free) =>
            amount < 1 || buy < 1 || free < 1 ? 0 : amount / (buy + free) * free;

        public static decimal DiscountedPrice(decimal price, int percentageOff) =>
            Math.Round(price * (1 - percentageOff / 100m), 2);

        public static decimal LineTotal(decimal price, int amount, SalesDto? sale) => sale?.SaleType switch
        {
            null => Total(price, amount),
            SaleTypeDto.MultiBuy => Total(price, amount - FreeItems(amount, sale.BuyAmount, sale.FreeAmount)),
            SaleTypeDto.SecondItemHalfPrice => SecondItemTotal(price, amount),
            SaleTypeDto.Bundle => BundleTotal(price, amount, sale.BuyAmount, sale.BundlePrice),
            _ => Total(DiscountedPrice(price, sale.PercentageOff), amount),
        };

        public static decimal SecondItemTotal(decimal price, int amount)
        {
            if (amount < 2) return Total(price, amount);
            int discounted = amount / 2;
            return Total(price, amount - discounted) + Total(DiscountedPrice(price, SecondItemPercentageOff), discounted);
        }

        public static decimal BundleTotal(decimal price, int amount, int bundleSize, decimal bundlePrice)
        {
            decimal normal = Total(price, amount);
            if (bundleSize < 2 || bundlePrice <= 0 || amount < bundleSize) return normal;

            decimal withBundles = Math.Round(amount / bundleSize * bundlePrice, 2) + Total(price, amount % bundleSize);
            return Math.Min(normal, withBundles);
        }

        public static decimal UnitPrice(decimal price, int amount, SalesDto? sale) =>
            amount < 1 ? price : Math.Round(LineTotal(price, amount, sale) / amount, 6);

        public static decimal Total(decimal unitPrice, int amount) => Math.Round(unitPrice * amount, 2);

        public static SalesDto? BestSale(ProductDto product, int amount, DateTime today, SalesDto? movingSale = null)
        {
            var sales = product.Sales
                .Where(s => s.SaleType != SaleTypeDto.Moving && IsActive(s, today))
                .ToList();

            if (sales.Count == 0)
                return movingSale is not null && !movingSale.SoftDeleted ? movingSale : null;

            return sales
                .OrderBy(s => LineTotal(product.Price, amount, s))
                .ThenByDescending(s => s.PercentageOff)
                .First();
        }

        public static string Describe(SalesDto sale) => sale.SaleType switch
        {
            SaleTypeDto.MultiBuy => $"{sale.BuyAmount}+{sale.FreeAmount}",
            SaleTypeDto.SecondItemHalfPrice => "2nd half price",
            SaleTypeDto.Bundle => $"{sale.BuyAmount} for €{sale.BundlePrice:0.00}",
            _ => $"-{sale.PercentageOff}%",
        };
    }
}
