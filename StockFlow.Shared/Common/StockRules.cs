namespace StockFlow.Shared.Common
{
    public static class ProductRules
    {
        public static bool OrderBlockActive(bool orderBlocked, DateTime? blockedUntil, DateTime today) =>
            orderBlocked && (blockedUntil is null || blockedUntil.Value.Date >= today.Date);

        public static string? SaleRefusal(string productName, bool isActive, bool isDraft, bool recalled)
        {
            if (recalled)
                return $"{productName} is recalled and can't be sold.";
            if (isDraft)
                return $"{productName} is still a draft and can't be sold yet.";
            if (!isActive)
                return $"{productName} is disabled and can't be sold.";
            return null;
        }

        public static string? OrderRefusal(string productName, bool isActive, bool orderable, bool recalled,
            bool orderBlocked, DateTime? blockedUntil, string? blockReason, DateTime today)
        {
            if (recalled)
                return $"{productName} is recalled and can't be ordered.";
            if (!isActive)
                return $"{productName} is not active and can't be ordered.";
            if (!orderable)
                return $"{productName} can't be ordered (non-orderable product).";
            if (OrderBlockActive(orderBlocked, blockedUntil, today))
            {
                string until = blockedUntil is { } date ? $" until {date:dd-MM-yyyy}" : "";
                string reason = string.IsNullOrWhiteSpace(blockReason) ? "" : $": {blockReason}";
                return $"{productName} is blocked for ordering{until}{reason}.";
            }
            return null;
        }
    }

    public static class PricePlanner
    {
        public static decimal CurrentPrice(IEnumerable<(DateTime ValidFrom, DateTime CreatedAt, decimal Price)> prices, decimal fallback, DateTime today) =>
            Current(prices, today)?.Price ?? fallback;

        public static (DateTime ValidFrom, DateTime CreatedAt, decimal Price)? Current(IEnumerable<(DateTime ValidFrom, DateTime CreatedAt, decimal Price)> prices, DateTime today)
        {
            var current = prices
                .Where(p => p.ValidFrom.Date <= today.Date)
                .OrderByDescending(p => p.ValidFrom.Date)
                .ThenByDescending(p => p.CreatedAt)
                .ToList();
            return current.Count == 0 ? null : current[0];
        }
    }

    public static class OrderDays
    {
        public static DateTime NextOrderDate(DayOfWeek orderDay, DateTime today)
        {
            int days = ((int)orderDay - (int)today.DayOfWeek + 7) % 7;
            return today.Date.AddDays(days == 0 ? 7 : days);
        }

        public static DateTime ExpectedDelivery(DateTime orderDate, int leadTimeDays) =>
            orderDate.Date.AddDays(Math.Max(0, leadTimeDays));
    }

    public static class Reorder
    {
        public static bool IsBelowMinimum(int stock, int reserved, int minimumStock) =>
            minimumStock > 0 && stock - reserved < minimumStock;

        public static int SuggestedAmount(int stock, int reserved, int onOrder, int minimumStock, int maximumStock)
        {
            if (minimumStock <= 0) return 0;
            int available = stock - reserved;
            if (available + onOrder >= minimumStock) return 0;
            int target = maximumStock > minimumStock ? maximumStock : minimumStock * 2;
            return Math.Max(1, target - available - onOrder);
        }
    }

    public static class ExpiryRules
    {
        public const int CountDaysBeforeExpiry = 7;

        public static bool NeedsCount(DateTime expiryDate, DateTime today) =>
            expiryDate.Date <= today.Date.AddDays(CountDaysBeforeExpiry);

        public static DateTime CountDueDate(DateTime expiryDate, DateTime today)
        {
            var due = expiryDate.Date.AddDays(-1);
            return due < today.Date ? today.Date : due;
        }
    }
}
