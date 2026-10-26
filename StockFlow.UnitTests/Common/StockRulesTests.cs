namespace StockFlow.UnitTests.Common
{
    public class StockRulesTests
    {
        private static readonly DateTime Today = new(2026, 10, 26);

        [Theory]
        [InlineData(true, null, true)]
        [InlineData(true, 0, true)]
        [InlineData(true, -1, false)]
        [InlineData(false, null, false)]
        public void OrderBlockActive_UntilDate_EndsAfterThatDay(bool blocked, int? untilOffset, bool expected)
        {
            DateTime? until = untilOffset is int offset ? Today.AddDays(offset) : null;

            Assert.Equal(expected, ProductRules.OrderBlockActive(blocked, until, Today));
        }

        [Fact]
        public void SaleRefusal_Recalled_SaysRecalled()
        {
            Assert.Equal("Hammer is recalled and can't be sold.", ProductRules.SaleRefusal("Hammer", isActive: true, isDraft: false, recalled: true));
        }

        [Fact]
        public void SaleRefusal_Draft_SaysDraft()
        {
            Assert.Contains("draft", ProductRules.SaleRefusal("Hammer", isActive: false, isDraft: true, recalled: false));
        }

        [Fact]
        public void SaleRefusal_Disabled_SaysDisabled()
        {
            Assert.Contains("disabled", ProductRules.SaleRefusal("Hammer", isActive: false, isDraft: false, recalled: false));
        }

        [Fact]
        public void SaleRefusal_Active_ReturnsNull()
        {
            Assert.Null(ProductRules.SaleRefusal("Hammer", isActive: true, isDraft: false, recalled: false));
        }

        [Fact]
        public void OrderRefusal_NonOrderable_ExplainsIt()
        {
            Assert.Contains("non-orderable", ProductRules.OrderRefusal("Hammer", true, false, false, false, null, null, Today));
        }

        [Fact]
        public void OrderRefusal_ActiveBlock_IncludesEndDateAndReason()
        {
            var result = ProductRules.OrderRefusal("Hammer", true, true, false, true, Today.AddDays(2), "Supplier out of stock", Today);

            Assert.Equal("Hammer is blocked for ordering until 28-10-2026: Supplier out of stock.", result);
        }

        [Fact]
        public void OrderRefusal_ExpiredBlock_ReturnsNull()
        {
            Assert.Null(ProductRules.OrderRefusal("Hammer", true, true, false, true, Today.AddDays(-1), "x", Today));
        }

        [Fact]
        public void CurrentPrice_PicksTheLatestStartedPrice()
        {
            var prices = new[]
            {
                (Today.AddDays(-10), Today.AddDays(-10), 10m),
                (Today.AddDays(-1), Today.AddDays(-2), 12m),
                (Today.AddDays(3), Today.AddDays(-1), 15m),
            };

            Assert.Equal(12m, PricePlanner.CurrentPrice(prices, 99m, Today));
            Assert.Equal(15m, PricePlanner.CurrentPrice(prices, 99m, Today.AddDays(3)));
        }

        [Fact]
        public void CurrentPrice_SameStartDay_LatestCreatedWins()
        {
            var prices = new[] { (Today, Today.AddHours(1), 10m), (Today, Today.AddHours(2), 11m) };

            Assert.Equal(11m, PricePlanner.CurrentPrice(prices, 99m, Today));
        }

        [Fact]
        public void CurrentPrice_NoStartedPrice_ReturnsFallback()
        {
            Assert.Equal(99m, PricePlanner.CurrentPrice([(Today.AddDays(1), Today, 5m)], 99m, Today));
        }

        [Theory]
        [InlineData(DayOfWeek.Monday, 6)]
        [InlineData(DayOfWeek.Tuesday, 7)]
        [InlineData(DayOfWeek.Friday, 3)]
        public void NextOrderDate_IsAlwaysAfterToday(DayOfWeek orderDay, int expectedDays)
        {
            var tuesday = new DateTime(2026, 10, 27);

            Assert.Equal(tuesday.AddDays(expectedDays), OrderDays.NextOrderDate(orderDay, tuesday));
        }

        [Fact]
        public void ExpectedDelivery_AddsLeadTime()
        {
            Assert.Equal(Today.AddDays(3), OrderDays.ExpectedDelivery(Today, 3));
        }

        [Theory]
        [InlineData(2, 0, 0, 5, 20, 18)]
        [InlineData(2, 0, 2, 5, 20, 16)]
        [InlineData(2, 0, 3, 5, 20, 0)]
        [InlineData(2, 0, 2, 5, 0, 6)]
        [InlineData(6, 2, 0, 5, 20, 16)]
        [InlineData(5, 0, 0, 5, 20, 0)]
        [InlineData(0, 0, 0, 0, 20, 0)]
        public void SuggestedAmount_FillsUpToTheMaximum(int stock, int reserved, int onOrder, int minimum, int maximum, int expected)
        {
            Assert.Equal(expected, Reorder.SuggestedAmount(stock, reserved, onOrder, minimum, maximum));
        }

        [Theory]
        [InlineData(4, 0, 5, true)]
        [InlineData(6, 2, 5, true)]
        [InlineData(5, 0, 5, false)]
        [InlineData(0, 0, 0, false)]
        public void IsBelowMinimum_UsesAvailableStock(int stock, int reserved, int minimum, bool expected)
        {
            Assert.Equal(expected, Reorder.IsBelowMinimum(stock, reserved, minimum));
        }

        [Theory]
        [InlineData(7, true)]
        [InlineData(8, false)]
        [InlineData(-1, true)]
        public void NeedsCount_OneWeekBeforeExpiry(int daysUntilExpiry, bool expected)
        {
            Assert.Equal(expected, ExpiryRules.NeedsCount(Today.AddDays(daysUntilExpiry), Today));
        }

        [Fact]
        public void CountDueDate_DayBeforeExpiryButNeverInThePast()
        {
            Assert.Equal(Today.AddDays(4), ExpiryRules.CountDueDate(Today.AddDays(5), Today));
            Assert.Equal(Today, ExpiryRules.CountDueDate(Today, Today));
        }

        [Theory]
        [InlineData("7000001", true)]
        [InlineData("700001", false)]
        [InlineData("8000001", false)]
        [InlineData("700000a", false)]
        [InlineData(null, false)]
        public void IsCustomerOrderNumber_SevenDigitsStartingWith7(string? input, bool expected)
        {
            Assert.Equal(expected, IdentifierFormats.IsCustomerOrderNumber(input));
        }
    }
}
