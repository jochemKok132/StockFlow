using StockFlow.Application.Interfaces.Ordering;
using StockFlow.Application.Interfaces.Repositories.Ordering;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Services;
using StockFlow.Application.Services.ManagementApp.Stock;
using StockFlow.Application.Services.Ordering;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Shared.DTOs.Ordering;
using StockFlow.Shared.DTOs.Stock.Supplier;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services
{
    public class SupplierServiceTests
    {
        private readonly Mock<ISupplierRepository> _suppliers = new();
        private readonly Mock<IProductRepository> _products = new();
        private readonly Mock<IPurchaseOrderService> _orders = new();
        private readonly SupplierService _sut;

        public SupplierServiceTests()
        {
            _sut = new SupplierService(_suppliers.Object, _products.Object, _orders.Object);
        }

        [Fact]
        public async Task SoftDeleteSupplierAsync_DefaultSupplier_ThrowsArgumentException()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.SoftDeleteSupplierAsync(Supplier.DefaultSupplierId));

            Assert.Equal(SupplierService.DefaultSupplierCantBeDeleted, exception.Message);
        }

        [Fact]
        public async Task SoftDeleteSupplierAsync_StillHasProducts_ThrowsArgumentException()
        {
            var supplier = TestData.Supplier();
            _suppliers.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);
            _products.Setup(r => r.CountProductsForSupplierAsync(supplier.Id)).ReturnsAsync(3);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.SoftDeleteSupplierAsync(supplier.Id));
            _suppliers.Verify(r => r.SoftDeleteAsync(It.IsAny<Supplier>()), Times.Never);
        }

        [Fact]
        public async Task SoftDeleteSupplierAsync_NoProducts_SoftDeletes()
        {
            var supplier = TestData.Supplier();
            _suppliers.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);

            await _sut.SoftDeleteSupplierAsync(supplier.Id);

            _suppliers.Verify(r => r.SoftDeleteAsync(supplier), Times.Once);
        }

        [Fact]
        public async Task UpdateSupplierAsync_NewOrderDay_ReschedulesOpenOrders()
        {
            var supplier = TestData.Supplier(orderDay: DayOfWeek.Monday);
            _suppliers.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);

            var result = await _sut.UpdateSupplierAsync(new UpdateSupplierDto { Id = supplier.Id, SupplierName = "Wholesale", OrderDay = DayOfWeek.Friday, LeadTimeDays = 1 });

            Assert.Equal(DayOfWeek.Friday, result.OrderDay);
            Assert.Equal(OrderDays.NextOrderDate(DayOfWeek.Friday, DateTime.Today), result.NextOrderDate);
            _orders.Verify(o => o.RescheduleOpenOrdersAsync(supplier.Id, DayOfWeek.Friday, DateTime.Today), Times.Once);
        }

        [Fact]
        public async Task UpdateSupplierAsync_SameOrderDay_DoesNotReschedule()
        {
            var supplier = TestData.Supplier(orderDay: DayOfWeek.Monday);
            _suppliers.Setup(r => r.GetByIdAsync(supplier.Id)).ReturnsAsync(supplier);

            await _sut.UpdateSupplierAsync(new UpdateSupplierDto { Id = supplier.Id, SupplierName = "New name", OrderDay = DayOfWeek.Monday });

            _orders.Verify(o => o.RescheduleOpenOrdersAsync(It.IsAny<Guid>(), It.IsAny<DayOfWeek>(), It.IsAny<DateTime>()), Times.Never);
        }
    }

    public class StockAutomationServiceTests
    {
        [Fact]
        public async Task RunAsync_RunsEveryStep()
        {
            var prices = new Mock<IProductControlService>();
            var counts = new Mock<IStockCountService>();
            var orders = new Mock<IPurchaseOrderService>();
            var today = new DateTime(2026, 10, 26);
            prices.Setup(p => p.ApplyDuePricesAsync(today)).ReturnsAsync(2);
            counts.Setup(c => c.CreateExpiryCountsAsync(today)).ReturnsAsync(1);
            orders.Setup(o => o.CreateAutomaticOrdersAsync(today)).ReturnsAsync(4);
            orders.Setup(o => o.SendDueOrdersAsync(today)).ReturnsAsync(3);

            var result = await new StockAutomationService(prices.Object, counts.Object, orders.Object).RunAsync(today);

            Assert.Equal(new StockAutomationResult(2, 1, 4, 3), result);
        }
    }

    public class CustomerOrderPickupTests
    {
        private readonly Mock<ICustomerOrderRepository> _orders = new();
        private readonly CustomerOrderService _sut;
        private readonly CustomerOrder _order;
        private readonly Guid _productId = Guid.NewGuid();

        public CustomerOrderPickupTests()
        {
            _sut = new CustomerOrderService(_orders.Object, new Mock<ICustomerRepository>().Object, new Mock<IProductRepository>().Object,
                new Mock<IProductService>().Object, new Mock<IPurchaseOrderService>().Object, new Mock<IStockAllocator>().Object);
            _order = new CustomerOrder
            {
                Id = Guid.NewGuid(),
                OrderNumber = 7000001,
                CustomerId = Guid.NewGuid(),
                Status = CustomerOrderStatus.ReadyForPickup,
                Lines = [new CustomerOrderLine { ProductId = _productId, Amount = 2, ReservedAmount = 2 }],
            };
            _orders.Setup(r => r.GetByNumberAsync(7000001)).ReturnsAsync(_order);
        }

        private Purchase PurchaseFor(Guid? customerId, int amount) => new()
        {
            CustomerId = customerId,
            CustomerOrderNumber = 7000001,
            ProductsSold = [new RegisterItemDetailDto { Product = new ProductDto { Id = _productId }, Amount = amount }],
        };

        [Fact]
        public async Task ValidatePickupAsync_RightCustomerAndProducts_DoesNotThrow()
        {
            Assert.Null(await Record.ExceptionAsync(() => _sut.ValidatePickupAsync(PurchaseFor(_order.CustomerId, 2))));
        }

        [Fact]
        public async Task ValidatePickupAsync_OtherCustomer_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ValidatePickupAsync(PurchaseFor(Guid.NewGuid(), 2)));
        }

        [Fact]
        public async Task ValidatePickupAsync_MissingProducts_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ValidatePickupAsync(PurchaseFor(_order.CustomerId, 1)));
        }

        [Fact]
        public async Task ValidatePickupAsync_StillWaiting_ThrowsArgumentException()
        {
            _order.Status = CustomerOrderStatus.Waiting;

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.ValidatePickupAsync(PurchaseFor(_order.CustomerId, 2)));
            Assert.Contains("waiting", exception.Message);
        }

        [Fact]
        public async Task MarkPickedUpAsync_StoresReceiptAndReleasesReservation()
        {
            await _sut.MarkPickedUpAsync(7000001, "9000000005");

            Assert.Equal(CustomerOrderStatus.PickedUp, _order.Status);
            Assert.Equal("9000000005", _order.ReceiptId);
            Assert.Equal(0, _order.Lines.Single().ReservedAmount);
            _orders.Verify(r => r.UpdateAsync(_order), Times.Once);
        }
    }

    public class StockAllocatorTests
    {
        private static CustomerOrder Order(int number, DateTime createdAt, Guid productId, int amount, int reserved = 0) => new()
        {
            Id = Guid.NewGuid(),
            OrderNumber = number,
            CreatedAt = createdAt,
            Status = CustomerOrderStatus.Waiting,
            Lines = [new CustomerOrderLine { ProductId = productId, Amount = amount, ReservedAmount = reserved }],
        };

        [Fact]
        public void Allocate_LimitedStock_OldestOrderFirst()
        {
            var product = Guid.NewGuid();
            var older = Order(7000001, new DateTime(2026, 10, 1), product, 3);
            var newer = Order(7000002, new DateTime(2026, 10, 2), product, 3);

            var changed = StockAllocator.Allocate([newer, older], new Dictionary<Guid, int> { [product] = 4 }, DateTime.UtcNow);

            Assert.Equal(CustomerOrderStatus.ReadyForPickup, older.Status);
            Assert.Equal(CustomerOrderStatus.Waiting, newer.Status);
            Assert.Equal(1, newer.Lines.Single().ReservedAmount);
            Assert.Equal(2, changed.Count);
        }

        [Fact]
        public void Allocate_NoStock_ChangesNothing()
        {
            var product = Guid.NewGuid();
            var order = Order(7000001, DateTime.Today, product, 3);

            var changed = StockAllocator.Allocate([order], new Dictionary<Guid, int> { [product] = 0 }, DateTime.UtcNow);

            Assert.Empty(changed);
            Assert.Equal(CustomerOrderStatus.Waiting, order.Status);
        }
    }

    public class ProductSalesStatisticsTests
    {
        [Fact]
        public async Task GetProductSalesAsync_GroupsSoldItemsPerDay()
        {
            var repository = new Mock<IStatisticsRepository>();
            var productId = Guid.NewGuid();
            var day = DateTime.Today.AddDays(-2);
            repository.Setup(r => r.GetProductLinesAsync(productId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync([(DateTime.SpecifyKind(day.AddHours(12), DateTimeKind.Local).ToUniversalTime(), 3, 1, 2.50m)]);

            var result = await new StatisticsService(repository.Object).GetProductSalesAsync(productId, DateTime.Today.AddDays(-6), DateTime.Today);

            Assert.Equal(7, result.Days.Count);
            var sold = result.Days.Single(d => d.Date == day);
            Assert.Equal(2, sold.ItemsSold);
            Assert.Equal(5.00m, sold.Revenue);
            Assert.Equal((3, 1), (result.ItemsSold, result.ItemsReturned));
        }

        [Fact]
        public async Task GetProductSalesAsync_EmptyProduct_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => new StatisticsService(new Mock<IStatisticsRepository>().Object).GetProductSalesAsync(Guid.Empty, null, null));
        }
    }

    public class OrderingEnumMappingTests
    {
        [Fact]
        public void EveryDomainValue_MapsToTheDtoWithTheSameName()
        {
            Assert.All(Enum.GetValues<ProductStatus>(), v => Assert.Equal(v.ToString(), v.ToDto().ToString()));
            Assert.All(Enum.GetValues<PurchaseOrderStatus>(), v => Assert.Equal(v, v.ToDto().ToDomain()));
            Assert.All(Enum.GetValues<CustomerOrderStatus>(), v => Assert.Equal(v, v.ToDto().ToDomain()));
            Assert.All(Enum.GetValues<StockCountStatus>(), v => Assert.Equal(v, v.ToDto().ToDomain()));
            Assert.All(Enum.GetValues<OrderLineSource>(), v => Assert.Equal(v.ToString(), v.ToDto().ToString()));
            Assert.All(Enum.GetValues<LogType>(), v => Assert.Equal(v.ToString(), v.ToDto().ToString()));
        }

        [Fact]
        public void UnknownValue_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ((ProductStatusDto)42).ToDomain());
        }
    }
}
