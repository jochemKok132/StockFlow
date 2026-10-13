using StockFlow.Shared.Common;

namespace StockFlow.UnitTests.Common
{
    public class ReturnRulesTests
    {
        [Theory]
        [InlineData(10, 10, 1)]
        [InlineData(5, 10, 0.5)]
        [InlineData(20, 10, 1)]
        [InlineData(5, 0, 0)]
        public void PaidShare_TotalsGiven_ReturnsClampedShare(double totalPaid, double linesTotal, double expected)
        {
            decimal result = ReturnRules.PaidShare((decimal)totalPaid, (decimal)linesTotal);

            Assert.Equal((decimal)expected, result);
        }

        [Fact]
        public void RefundFor_NoPointsUsed_ReturnsFullPrice()
        {
            decimal result = ReturnRules.RefundFor(2.50m, 2, 10m, 10m);

            Assert.Equal(5.00m, result);
        }

        [Fact]
        public void RefundFor_PointsCoveredPart_ReturnsProportionalRefund()
        {
            decimal result = ReturnRules.RefundFor(10m, 1, 8m, 10m);

            Assert.Equal(8.00m, result);
        }

        [Fact]
        public void RefundFor_HalfCent_RoundsAwayFromZero()
        {
            decimal result = ReturnRules.RefundFor(0.99m, 1, 5m, 10m);

            Assert.Equal(0.50m, result);
        }
    }
}
