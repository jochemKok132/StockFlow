using StockFlow.Application.Services;

namespace StockFlow.UnitTests.Services
{
    public class StatisticsServiceTests
    {
        private readonly Mock<IStatisticsRepository> _repository = new();
        private readonly StatisticsService _sut;

        public StatisticsServiceTests()
        {
            _sut = new StatisticsService(_repository.Object);
        }

        private static CashRegisterLogs Purchase(DateTime localTime, double total, Guid? customerId, Product product, int amount, int returned = 0) => new()
        {
            Id = Guid.NewGuid(),
            ReceiptId = "9000000001",
            CreatedAt = DateTime.SpecifyKind(localTime, DateTimeKind.Local).ToUniversalTime(),
            TotalPrice = total,
            TotalOff = 1,
            PointsUsed = 10,
            PointsEarned = 2,
            CustomerId = customerId,
            ProductsSold = [new RegisterItemDetail { Id = Guid.NewGuid(), Product = product, ProductId = product.Id, Amount = amount, ReturnedAmount = returned, PriceAtTimeOfSale = (decimal)total / amount }],
        };

        [Fact]
        public async Task GetOverviewAsync_Purchases_AddsUpTotalsAndPercentages()
        {
            var hammer = new Product { Id = Guid.NewGuid(), Barcode = 500000001, ProductName = "Hamer", Description = "x" };
            var saw = new Product { Id = Guid.NewGuid(), Barcode = 500000002, ProductName = "Zaag", Description = "x" };
            var day = new DateTime(2026, 10, 15);
            _repository.Setup(r => r.GetPurchasesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(
            [
                Purchase(day.AddHours(10), 10, Guid.NewGuid(), hammer, 2),
                Purchase(day.AddHours(11), 20, null, saw, 1),
                Purchase(day.AddDays(1).AddHours(9), 30, Guid.NewGuid(), hammer, 3, returned: 1),
                Purchase(day.AddDays(1).AddHours(15), 40, Guid.NewGuid(), saw, 4),
            ]);
            _repository.Setup(r => r.CountNewCustomersAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(3);

            var result = await _sut.GetOverviewAsync(day, day.AddDays(2));

            Assert.Equal(4, result.Purchases);
            Assert.Equal(100m, result.Revenue);
            Assert.Equal(25m, result.AverageReceipt);
            Assert.Equal(3, result.PurchasesWithCustomer);
            Assert.Equal(75.0, result.CustomerCardPercentage);
            Assert.Equal(10, result.ItemsSold);
            Assert.Equal(1, result.ItemsReturned);
            Assert.Equal(4m, result.Discounts);
            Assert.Equal(40, result.PointsUsed);
            Assert.Equal(8, result.PointsEarned);
            Assert.Equal(3, result.NewCustomers);
            Assert.Equal(3, result.Days.Count);
            Assert.Equal(30m, result.Days[0].Revenue);
            Assert.Equal(50.0, result.Days[0].CustomerCardPercentage);
            Assert.Equal(70m, result.Days[1].Revenue);
            Assert.Equal(0, result.Days[2].Purchases);
            Assert.Equal("Zaag", result.TopProducts[0].ProductName);
            Assert.Equal(5, result.TopProducts[0].ItemsSold);
            Assert.Equal(4, result.TopProducts[1].ItemsSold);
        }

        [Fact]
        public async Task GetOverviewAsync_NoPurchases_ReturnsZeroesWithoutDividingByZero()
        {
            _repository.Setup(r => r.GetPurchasesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync([]);

            var result = await _sut.GetOverviewAsync(new DateTime(2026, 10, 1), new DateTime(2026, 10, 7));

            Assert.Equal(0m, result.AverageReceipt);
            Assert.Equal(0.0, result.CustomerCardPercentage);
            Assert.Equal(7, result.Days.Count);
            Assert.Empty(result.TopProducts);
        }

        [Fact]
        public async Task GetOverviewAsync_NoDates_UsesTheLast30Days()
        {
            _repository.Setup(r => r.GetPurchasesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync([]);

            var result = await _sut.GetOverviewAsync(null, null);

            Assert.Equal(DateTime.Today, result.To);
            Assert.Equal(DateTime.Today.AddDays(-29), result.From);
            Assert.Equal(30, result.Days.Count);
        }

        [Fact]
        public async Task GetOverviewAsync_EndBeforeStart_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetOverviewAsync(new DateTime(2026, 10, 2), new DateTime(2026, 10, 1)));
        }

        [Fact]
        public async Task GetOverviewAsync_MoreThanAYear_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetOverviewAsync(new DateTime(2025, 1, 1), new DateTime(2026, 1, 2)));
        }
    }
}
