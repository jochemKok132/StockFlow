namespace StockFlow.Application.Common
{
    public static class ReturnRules
    {
        public static decimal PaidShare(decimal totalPaid, decimal linesTotal) =>
            linesTotal <= 0 ? 0m : Math.Clamp(totalPaid / linesTotal, 0m, 1m);

        public static decimal RefundFor(decimal pricePerItem, int amount, decimal totalPaid, decimal linesTotal) =>
            Math.Round(pricePerItem * amount * PaidShare(totalPaid, linesTotal), 2, MidpointRounding.AwayFromZero);
    }
}
