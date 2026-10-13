namespace StockFlow.Shared.Common
{
    public static class LoyaltyPoints
    {
        public const int PointsPerEuro = 100;
        public const int PointsEarnedPerEuro = 1;
        public const int WelcomePoints = 250;

        public static decimal ToEuros(int points) => (decimal)points / PointsPerEuro;

        public static int PointsEarned(decimal amountPaid) =>
            amountPaid <= 0 ? 0 : (int)Math.Floor(amountPaid) * PointsEarnedPerEuro;

        public static int Redeemable(int availablePoints, decimal amountToPay)
        {
            if (availablePoints <= 0 || amountToPay <= 0)
                return 0;

            int maxForAmount = (int)Math.Floor(amountToPay * PointsPerEuro);
            return Math.Min(availablePoints, maxForAmount);
        }
    }
}
