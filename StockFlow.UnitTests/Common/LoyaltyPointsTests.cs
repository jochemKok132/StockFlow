using StockFlow.Shared.Common;

namespace StockFlow.UnitTests.Common
{
    public class LoyaltyPointsTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(0.99, 0)]
        [InlineData(1.99, 1)]
        [InlineData(2.00, 2)]
        [InlineData(25.50, 25)]
        public void PointsEarned_AmountPaid_ReturnsOnePointPerWholeEuro(double amountPaid, int expected)
        {
            int result = LoyaltyPoints.PointsEarned((decimal)amountPaid);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void PointsEarned_NegativeAmount_ReturnsZero()
        {
            int result = LoyaltyPoints.PointsEarned(-5m);

            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData(1250, 8.40, 840)]
        [InlineData(50, 10.00, 50)]
        [InlineData(1000, 0.05, 5)]
        [InlineData(840, 8.40, 840)]
        public void Redeemable_PointsAndAmount_ReturnsAllOrNothingAmount(int available, double amount, int expected)
        {
            int result = LoyaltyPoints.Redeemable(available, (decimal)amount);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(0, 10.00)]
        [InlineData(100, 0)]
        [InlineData(-10, 10.00)]
        public void Redeemable_NoPointsOrNothingToPay_ReturnsZero(int available, double amount)
        {
            int result = LoyaltyPoints.Redeemable(available, (decimal)amount);

            Assert.Equal(0, result);
        }

        [Fact]
        public void ToEuros_250Points_ReturnsTwoFifty()
        {
            decimal result = LoyaltyPoints.ToEuros(250);

            Assert.Equal(2.50m, result);
        }

        [Fact]
        public void WelcomePoints_NewAccount_Is250()
        {
            Assert.Equal(250, LoyaltyPoints.WelcomePoints);
        }
    }
}
