using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.Ordering;
using StockFlow.Application.Services.CashRegister;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services
{
    public class RegisterDataServiceTests
    {
        private const string ReceiptId = "9000000001";

        private readonly Mock<IProductService> _productService = new();
        private readonly Mock<ICashRegisterLogsService> _cashRegisterLogsService = new();
        private readonly Mock<ICustomerRepository> _customerRepository = new();
        private readonly Mock<ICustomerLogsService> _customerLogsService = new();
        private readonly Mock<ICashRegisterLogsRepository> _cashRegisterLogsRepository = new();
        private readonly Mock<ISalesRepository> _salesRepository = new();
        private readonly Mock<ICustomerOrderService> _customerOrderService = new();
        private readonly Mock<IReturnLogsRepository> _returnLogsRepository = new();
        private readonly RegisterDataService _sut;
        private readonly Guid _employeeId = Guid.NewGuid();

        public RegisterDataServiceTests()
        {
            _sut = new RegisterDataService(_productService.Object, _cashRegisterLogsService.Object, _customerRepository.Object, _customerLogsService.Object, _cashRegisterLogsRepository.Object, _salesRepository.Object, _customerOrderService.Object, _returnLogsRepository.Object);
            _cashRegisterLogsService.Setup(s => s.CreateCashRegisterLogsAsync(It.IsAny<CreateCashRegisterLogsDto>())).ReturnsAsync(ReceiptId);
        }

        [Fact]
        public async Task GetCustomerAsync_ActiveMovingSale_AddsTheMovingSale()
        {
            var customer = TestData.Customer();
            customer.MovingSaleUntil = DateTime.Today.AddDays(5);
            var movingSale = new Sales { Id = SalePricing.MovingSaleId, SaleName = SalePricing.MovingSaleName, Description = "x", SaleType = SaleType.Moving, PercentageOff = 10 };
            _customerRepository.Setup(r => r.GetCustomerByCustomerIdAsync(customer.CustomerId)).ReturnsAsync(customer);
            _salesRepository.Setup(r => r.GetByIdAsync(SalePricing.MovingSaleId)).ReturnsAsync(movingSale);

            var result = await _sut.GetCustomerAsync(customer.CustomerId);

            Assert.NotNull(result.MovingSale);
            Assert.Equal(10, result.MovingSale.PercentageOff);
        }

        [Fact]
        public async Task GetCustomerAsync_EndedMovingSale_HasNoMovingSale()
        {
            var customer = TestData.Customer();
            customer.MovingSaleUntil = DateTime.Today.AddDays(-1);
            _customerRepository.Setup(r => r.GetCustomerByCustomerIdAsync(customer.CustomerId)).ReturnsAsync(customer);

            var result = await _sut.GetCustomerAsync(customer.CustomerId);

            Assert.Null(result.MovingSale);
            _salesRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetCustomerAsync_NotACustomerId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetCustomerAsync("70000001"));
        }

        [Fact]
        public async Task FindCustomersByAddressAsync_ValidAddress_ReturnsCustomers()
        {
            _customerRepository.Setup(r => r.GetCustomersByAddressAsync("1234 AB", "12")).ReturnsAsync(new[] { TestData.Customer(), TestData.Customer() });

            var result = await _sut.FindCustomersByAddressAsync("1234 AB", "12");

            Assert.Equal(2, result.Count);
        }

        [Theory]
        [InlineData("12", "12")]
        [InlineData("1234 AB", "")]
        public async Task FindCustomersByAddressAsync_InvalidAddress_ThrowsArgumentException(string postalCode, string houseNumber)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.FindCustomersByAddressAsync(postalCode, houseNumber));
        }

        private static RegisterItemDetailDto Line(Guid productId, decimal price, int amount) => new()
        {
            Product = new ProductDto { Id = productId, ProductName = "Hammer" },
            PriceAtTimeOfSale = price,
            Amount = amount,
        };

        private static Purchase Purchase(Guid? customerId, int pointsUsed, params RegisterItemDetailDto[] lines) => new()
        {
            CustomerId = customerId,
            PointsUsed = pointsUsed,
            TotalPrice = (double)(lines.Sum(l => l.PriceAtTimeOfSale * l.Amount) - LoyaltyPoints.ToEuros(pointsUsed)),
            ProductsSold = lines,
        };

        [Fact]
        public async Task FinalizePurchase_UnsellableProduct_StopsBeforeChangingStock()
        {
            _productService.Setup(p => p.EnsureSellableAsync(It.IsAny<IEnumerable<Guid>>())).ThrowsAsync(new ArgumentException("Hammer is recalled and can't be sold."));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.FinalizePurchase(Purchase(null, 0, Line(Guid.NewGuid(), 10m, 1)), _employeeId));

            _productService.Verify(p => p.SellAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task FinalizePurchase_CustomerOrder_ValidatesAndMarksItPickedUp()
        {
            var purchase = Purchase(null, 0, Line(Guid.NewGuid(), 10m, 1));
            purchase.CustomerOrderNumber = 7000001;

            await _sut.FinalizePurchase(purchase, _employeeId);

            _customerOrderService.Verify(s => s.ValidatePickupAsync(purchase), Times.Once);
            _customerOrderService.Verify(s => s.MarkPickedUpAsync(7000001, ReceiptId), Times.Once);
        }

        [Fact]
        public async Task FinalizePurchase_InvalidPickup_DoesNotSell()
        {
            var purchase = Purchase(null, 0, Line(Guid.NewGuid(), 10m, 1));
            purchase.CustomerOrderNumber = 7000001;
            _customerOrderService.Setup(s => s.ValidatePickupAsync(purchase)).ThrowsAsync(new ArgumentException("not ready"));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.FinalizePurchase(purchase, _employeeId));

            _cashRegisterLogsService.Verify(s => s.CreateCashRegisterLogsAsync(It.IsAny<CreateCashRegisterLogsDto>()), Times.Never);
        }

        [Fact]
        public async Task FinalizePurchase_NullPurchase_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.FinalizePurchase(null!, _employeeId));
        }

        [Fact]
        public async Task FinalizePurchase_WalkIn_LowersStockPerLineAndReturnsReceiptId()
        {
            var hammer = Guid.NewGuid();
            var saw = Guid.NewGuid();
            CreateCashRegisterLogsDto? log = null;
            _cashRegisterLogsService.Setup(s => s.CreateCashRegisterLogsAsync(It.IsAny<CreateCashRegisterLogsDto>()))
                .Callback<CreateCashRegisterLogsDto>(l => log = l).ReturnsAsync(ReceiptId);

            var result = await _sut.FinalizePurchase(Purchase(null, 0, Line(hammer, 10m, 2), Line(saw, 5m, 1)), _employeeId);

            Assert.Equal(ReceiptId, result);
            _productService.Verify(p => p.SellAsync(hammer, 2, _employeeId, ReceiptId), Times.Once);
            _productService.Verify(p => p.SellAsync(saw, 1, _employeeId, ReceiptId), Times.Once);
            Assert.Equal(0, log?.PointsEarned);
            _customerRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task FinalizePurchase_SoftDeletedCustomer_ThrowsArgumentException()
        {
            var customer = TestData.Customer(softDeleted: true);
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.FinalizePurchase(Purchase(customer.Id, 0, Line(Guid.NewGuid(), 10m, 1)), _employeeId));
            _productService.Verify(p => p.SellAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task FinalizePurchase_CustomerWithoutPoints_EarnsOnePointPerWholeEuro()
        {
            var customer = TestData.Customer(savedPoints: 0);
            CreateCashRegisterLogsDto? log = null;
            CreateCustomerLogsDto? customerLog = null;
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            _cashRegisterLogsService.Setup(s => s.CreateCashRegisterLogsAsync(It.IsAny<CreateCashRegisterLogsDto>()))
                .Callback<CreateCashRegisterLogsDto>(l => log = l).ReturnsAsync(ReceiptId);
            _customerLogsService.Setup(s => s.CreateCustomerLogsAsync(It.IsAny<CreateCustomerLogsDto>())).Callback<CreateCustomerLogsDto>(l => customerLog = l);

            await _sut.FinalizePurchase(Purchase(customer.Id, 0, Line(Guid.NewGuid(), 1.99m, 2)), _employeeId);

            Assert.Equal(3, customer.SavedPoints);
            Assert.Equal(3, log?.PointsEarned);
            _customerRepository.Verify(r => r.UpdateAsync(customer), Times.Once);
            Assert.NotNull(customerLog);
            Assert.Contains("SavedPoints: New: 3. Old: 0", customerLog.Messages);
            Assert.Contains($"Receipt: {ReceiptId}", customerLog.Messages);
            Assert.Contains("PointsEarned: 3", customerLog.Messages);
        }

        [Fact]
        public async Task FinalizePurchase_AllPointsUsed_SubtractsUsedAndAddsEarnedOnRemainder()
        {
            var customer = TestData.Customer(savedPoints: 1000);
            CreateCustomerLogsDto? customerLog = null;
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            _customerLogsService.Setup(s => s.CreateCustomerLogsAsync(It.IsAny<CreateCustomerLogsDto>())).Callback<CreateCustomerLogsDto>(l => customerLog = l);

            await _sut.FinalizePurchase(Purchase(customer.Id, 1000, Line(Guid.NewGuid(), 12.50m, 1)), _employeeId);

            Assert.Equal(2, customer.SavedPoints);
            Assert.NotNull(customerLog);
            Assert.Contains(customerLog.Messages, m => m.StartsWith("PointsUsed: 1000"));
            Assert.Contains("PointsEarned: 2", customerLog.Messages);
        }

        [Fact]
        public async Task FinalizePurchase_PartialPoints_ThrowsArgumentException()
        {
            var customer = TestData.Customer(savedPoints: 1000);
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.FinalizePurchase(Purchase(customer.Id, 500, Line(Guid.NewGuid(), 12.50m, 1)), _employeeId));
        }

        [Fact]
        public async Task FinalizePurchase_PointsCancelOut_StillLogsUnchangedBalance()
        {
            var customer = TestData.Customer(savedPoints: 50);
            CreateCustomerLogsDto? customerLog = null;
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            _customerLogsService.Setup(s => s.CreateCustomerLogsAsync(It.IsAny<CreateCustomerLogsDto>())).Callback<CreateCustomerLogsDto>(l => customerLog = l);

            await _sut.FinalizePurchase(Purchase(customer.Id, 50, Line(Guid.NewGuid(), 50.50m, 1)), _employeeId);

            Assert.Equal(50, customer.SavedPoints);
            _customerRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
            Assert.NotNull(customerLog);
            Assert.Contains("SavedPoints: Unchanged (50)", customerLog.Messages);
        }

        private CashRegisterLogs Receipt(Guid productId, int amount, int returned, decimal price, int pointsEarned = 0, Guid? customerId = null, double? totalPaid = null)
        {
            var log = new CashRegisterLogs
            {
                Id = Guid.NewGuid(),
                ReceiptId = ReceiptId,
                CustomerId = customerId,
                PointsEarned = pointsEarned,
                TotalPrice = totalPaid ?? (double)(price * amount),
                ProductsSold =
                [
                    new RegisterItemDetail { Id = Guid.NewGuid(), ProductId = productId, Product = TestData.Product(), Amount = amount, ReturnedAmount = returned, PriceAtTimeOfSale = price },
                ],
            };
            _cashRegisterLogsRepository.Setup(r => r.GetByReceiptIdAsync(ReceiptId, true)).ReturnsAsync(log);
            return log;
        }

        private static ReturnRequest ReturnOf(Guid productId, int amount) => new()
        {
            ReceiptId = ReceiptId,
            Items = [new ReturnItem { ProductId = productId, Amount = amount }],
        };

        [Fact]
        public async Task ProcessReturn_InvalidReceiptId_ThrowsArgumentException()
        {
            var request = new ReturnRequest { ReceiptId = "123", Items = [new ReturnItem { ProductId = Guid.NewGuid(), Amount = 1 }] };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ProcessReturn(request, _employeeId));
        }

        [Fact]
        public async Task ProcessReturn_ReceiptNotFound_ThrowsKeyNotFoundException()
        {
            _cashRegisterLogsRepository.Setup(r => r.GetByReceiptIdAsync(ReceiptId, true)).ReturnsAsync((CashRegisterLogs?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.ProcessReturn(ReturnOf(Guid.NewGuid(), 1), _employeeId));
        }

        [Fact]
        public async Task ProcessReturn_ProductNotOnReceipt_ThrowsArgumentException()
        {
            Receipt(Guid.NewGuid(), 2, 0, 5m);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ProcessReturn(ReturnOf(Guid.NewGuid(), 1), _employeeId));
        }

        [Fact]
        public async Task ProcessReturn_MoreThanStillReturnable_ThrowsArgumentException()
        {
            var productId = Guid.NewGuid();
            Receipt(productId, 3, 2, 5m);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ProcessReturn(ReturnOf(productId, 2), _employeeId));
            _cashRegisterLogsRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ProcessReturn_AlreadyFullyReturned_ThrowsArgumentException()
        {
            var productId = Guid.NewGuid();
            Receipt(productId, 2, 2, 5m);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ProcessReturn(ReturnOf(productId, 1), _employeeId));
        }

        [Fact]
        public async Task ProcessReturn_ValidReturn_MarksReturnedRaisesStockAndRefunds()
        {
            var productId = Guid.NewGuid();
            var receipt = Receipt(productId, 3, 1, 4m);

            var result = await _sut.ProcessReturn(ReturnOf(productId, 2), _employeeId);

            Assert.Equal(3, receipt.ProductsSold.Single().ReturnedAmount);
            Assert.Equal(8.00m, result.RefundAmount);
            Assert.Equal(2, result.ItemsReturned);
            Assert.Equal(ReceiptId, result.ReceiptId);
            _cashRegisterLogsRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _productService.Verify(p => p.UpdateProductStockAsync(productId, 2, _employeeId, LogTypeDto.Returned), Times.Once);
        }

        [Fact]
        public async Task ProcessReturn_ReceiptPaidPartlyWithPoints_RefundsProportionally()
        {
            var productId = Guid.NewGuid();
            Receipt(productId, 2, 0, 10m, totalPaid: 15);

            var result = await _sut.ProcessReturn(ReturnOf(productId, 1), _employeeId);

            Assert.Equal(7.50m, result.RefundAmount);
        }

        [Fact]
        public async Task ProcessReturn_CustomerEarnedPoints_DeductsPointsForReturnedItems()
        {
            var productId = Guid.NewGuid();
            var customer = TestData.Customer(savedPoints: 20);
            var receipt = Receipt(productId, 3, 0, 4m, pointsEarned: 12, customerId: customer.Id);
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

            var result = await _sut.ProcessReturn(ReturnOf(productId, 2), _employeeId);

            Assert.Equal(8, result.PointsDeducted);
            Assert.Equal(4, receipt.PointsEarned);
            Assert.Equal(12, customer.SavedPoints);
            _customerLogsService.Verify(s => s.CreateCustomerLogsAsync(It.Is<CreateCustomerLogsDto>(l => l.Messages.Contains("PointsDeducted: 8"))), Times.Once);
        }

        [Fact]
        public async Task ProcessReturn_CustomerSpentEarnedPoints_NeverGoesBelowZero()
        {
            var productId = Guid.NewGuid();
            var customer = TestData.Customer(savedPoints: 3);
            Receipt(productId, 1, 0, 10m, pointsEarned: 10, customerId: customer.Id);
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

            await _sut.ProcessReturn(ReturnOf(productId, 1), _employeeId);

            Assert.Equal(0, customer.SavedPoints);
        }

        [Fact]
        public async Task ProcessReturn_ValidReturn_SavesOneReturnLogWithLines()
        {
            var productId = Guid.NewGuid();
            var customer = TestData.Customer(savedPoints: 20);
            var receipt = Receipt(productId, 3, 0, 4m, pointsEarned: 12, customerId: customer.Id);
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            ReturnLogs? saved = null;
            _returnLogsRepository.Setup(r => r.AddAsync(It.IsAny<ReturnLogs>())).Callback<ReturnLogs>(l => saved = l);

            await _sut.ProcessReturn(ReturnOf(productId, 2), _employeeId);

            Assert.NotNull(saved);
            Assert.Equal(ReceiptId, saved.ReceiptId);
            Assert.Equal(receipt.Id, saved.CashRegisterLogId);
            Assert.Equal(_employeeId, saved.EmployeeId);
            Assert.Equal(customer.Id, saved.CustomerId);
            Assert.Equal(8.00m, saved.RefundAmount);
            Assert.Equal(8, saved.PointsDeducted);
            var line = Assert.Single(saved.Lines);
            Assert.Equal(productId, line.ProductId);
            Assert.Equal(2, line.Amount);
            Assert.Equal(4m, line.PriceAtTimeOfSale);
            Assert.Equal(8.00m, line.RefundAmount);
        }

        [Fact]
        public async Task ProcessReturn_InvalidReturn_SavesNoReturnLog()
        {
            var productId = Guid.NewGuid();
            Receipt(productId, 2, 2, 5m);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ProcessReturn(ReturnOf(productId, 1), _employeeId));

            _returnLogsRepository.Verify(r => r.AddAsync(It.IsAny<ReturnLogs>()), Times.Never);
        }
    }
}
