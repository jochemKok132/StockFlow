namespace StockFlow.UnitTests.Common
{
    public class SalePricingTests
    {
        private static readonly DateTime Today = new(2026, 10, 20);

        private static SalesDto Percentage(int percentage, DateTime? start = null, DateTime? end = null) => new()
        {
            Id = Guid.NewGuid(),
            SaleName = "Korting",
            Description = "x",
            SaleType = SaleTypeDto.Percentage,
            PercentageOff = percentage,
            StartDate = start,
            EndDate = end,
        };

        private static SalesDto MultiBuy(int buy, int free) => new()
        {
            Id = Guid.NewGuid(),
            SaleName = $"{buy}+{free}",
            Description = "x",
            SaleType = SaleTypeDto.MultiBuy,
            BuyAmount = buy,
            FreeAmount = free,
        };

        private static SalesDto SecondHalf() => new()
        {
            Id = Guid.NewGuid(),
            SaleName = "2e halve prijs",
            Description = "x",
            SaleType = SaleTypeDto.SecondItemHalfPrice,
        };

        private static SalesDto Bundle(int size, decimal price) => new()
        {
            Id = Guid.NewGuid(),
            SaleName = $"{size} voor {price}",
            Description = "x",
            SaleType = SaleTypeDto.Bundle,
            BuyAmount = size,
            BundlePrice = price,
        };

        private static SalesDto Moving() => new()
        {
            Id = SalePricing.MovingSaleId,
            SaleName = SalePricing.MovingSaleName,
            Description = "x",
            SaleType = SaleTypeDto.Moving,
            PercentageOff = 10,
        };

        [Theory]
        [InlineData(1, 1, 1, 0)]
        [InlineData(2, 1, 1, 1)]
        [InlineData(3, 1, 1, 1)]
        [InlineData(4, 1, 1, 2)]
        [InlineData(2, 2, 1, 0)]
        [InlineData(3, 2, 1, 1)]
        [InlineData(7, 2, 1, 2)]
        [InlineData(5, 0, 1, 0)]
        public void FreeItems_ReturnsFreeItemsPerGroup(int amount, int buy, int free, int expected)
        {
            Assert.Equal(expected, SalePricing.FreeItems(amount, buy, free));
        }

        [Fact]
        public void LineTotal_OnePlusOneThreeItems_PaysForTwo()
        {
            var result = SalePricing.LineTotal(3.00m, 3, MultiBuy(1, 1));

            Assert.Equal(6.00m, result);
        }

        [Fact]
        public void LineTotal_TwoPlusOneSixItems_PaysForFour()
        {
            var result = SalePricing.LineTotal(2.50m, 6, MultiBuy(2, 1));

            Assert.Equal(10.00m, result);
        }

        [Fact]
        public void LineTotal_Percentage_RoundsTheItemPrice()
        {
            var result = SalePricing.LineTotal(9.99m, 3, Percentage(15));

            Assert.Equal(25.47m, result);
        }

        [Fact]
        public void UnitPrice_TimesAmount_GivesTheLineTotalBackForBigAmounts()
        {
            var sale = MultiBuy(2, 1);

            for (int amount = 1; amount <= 999; amount++)
            {
                decimal unit = SalePricing.UnitPrice(3.33m, amount, sale);
                Assert.Equal(SalePricing.LineTotal(3.33m, amount, sale), SalePricing.Total(unit, amount));
            }
        }

        [Theory]
        [InlineData(null, null, true)]
        [InlineData("2026-10-20", "2026-10-20", true)]
        [InlineData("2026-10-21", null, false)]
        [InlineData(null, "2026-10-19", false)]
        [InlineData("2026-10-01", "2026-10-31", true)]
        public void IsInPeriod_ChecksStartAndEndInclusive(string? start, string? end, bool expected)
        {
            DateTime? from = start is null ? null : DateTime.Parse(start);
            DateTime? to = end is null ? null : DateTime.Parse(end);

            Assert.Equal(expected, SalePricing.IsInPeriod(from, to, Today));
        }

        [Fact]
        public void Status_FutureStart_IsScheduled()
        {
            Assert.Equal(SaleStatusDto.Scheduled, SalePricing.Status(Today.AddDays(1), null, Today));
        }

        [Fact]
        public void Status_PastEnd_IsEnded()
        {
            Assert.Equal(SaleStatusDto.Ended, SalePricing.Status(null, Today.AddDays(-1), Today));
        }

        [Fact]
        public void BestSale_PicksTheCheapestActiveSale()
        {
            var tenPercent = Percentage(10);
            var onePlusOne = MultiBuy(1, 1);
            var product = new ProductDto { Price = 4.00m, Sales = [tenPercent, onePlusOne] };

            Assert.Same(tenPercent, SalePricing.BestSale(product, 1, Today));
            Assert.Same(onePlusOne, SalePricing.BestSale(product, 2, Today));
        }

        [Fact]
        public void BestSale_EndedSale_IsIgnored()
        {
            var product = new ProductDto { Price = 4.00m, Sales = [Percentage(50, end: Today.AddDays(-1))] };

            Assert.Null(SalePricing.BestSale(product, 1, Today));
        }

        [Fact]
        public void BestSale_NoProductSale_UsesMovingSale()
        {
            var moving = Moving();
            var product = new ProductDto { Price = 4.00m };

            Assert.Same(moving, SalePricing.BestSale(product, 1, Today, moving));
        }

        [Fact]
        public void BestSale_ProductAlreadyOnSale_SkipsMovingSale()
        {
            var fivePercent = Percentage(5);
            var product = new ProductDto { Price = 4.00m, Sales = [fivePercent] };

            Assert.Same(fivePercent, SalePricing.BestSale(product, 1, Today, Moving()));
        }

        [Fact]
        public void IsMovingSaleActive_UntilToday_IsActive()
        {
            Assert.True(SalePricing.IsMovingSaleActive(Today, Today));
            Assert.False(SalePricing.IsMovingSaleActive(Today.AddDays(-1), Today));
            Assert.False(SalePricing.IsMovingSaleActive(null, Today));
        }

        [Fact]
        public void Describe_ShowsPercentageOrMultiBuy()
        {
            Assert.Equal("-15%", SalePricing.Describe(Percentage(15)));
            Assert.Equal("2+1", SalePricing.Describe(MultiBuy(2, 1)));
            Assert.Equal("2nd half price", SalePricing.Describe(SecondHalf()));
            Assert.StartsWith("3 for €10", SalePricing.Describe(Bundle(3, 10m)));
        }

        [Theory]
        [InlineData(1, 10.00)]
        [InlineData(2, 15.00)]
        [InlineData(3, 25.00)]
        [InlineData(4, 30.00)]
        public void LineTotal_SecondItemHalfPrice_EveryOtherItemHalf(int amount, double expected)
        {
            var result = SalePricing.LineTotal(10m, amount, SecondHalf());

            Assert.Equal((decimal)expected, result);
        }

        [Fact]
        public void LineTotal_SecondItemHalfPrice_RoundsTheHalfPrice()
        {
            var result = SalePricing.LineTotal(2.99m, 2, SecondHalf());

            Assert.Equal(4.49m, result);
        }

        [Theory]
        [InlineData(2, 8.00)]
        [InlineData(3, 10.00)]
        [InlineData(4, 14.00)]
        [InlineData(6, 20.00)]
        [InlineData(7, 24.00)]
        public void LineTotal_ThreeForTen_BundlesAndRest(int amount, double expected)
        {
            var result = SalePricing.LineTotal(4m, amount, Bundle(3, 10m));

            Assert.Equal((decimal)expected, result);
        }

        [Fact]
        public void LineTotal_BundleMoreExpensiveThanNormal_KeepsNormalPrice()
        {
            var result = SalePricing.LineTotal(2m, 3, Bundle(3, 10m));

            Assert.Equal(6m, result);
        }

        [Fact]
        public void BestSale_BundleCheaperThanPercentage_PicksBundle()
        {
            var bundle = Bundle(3, 10m);
            var product = new ProductDto { Price = 4m, Sales = [Percentage(10), bundle] };

            var result = SalePricing.BestSale(product, 3, Today);

            Assert.Same(bundle, result);
        }
    }
}
