using StockFlow.Application.Services.ManagementApp.AuditLogs;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services.AuditLogs
{
    public class CashRegisterLogsServiceTests
    {
        private readonly Mock<ICashRegisterLogsRepository> _repository = new();
        private readonly CashRegisterLogsService _sut;

        public CashRegisterLogsServiceTests()
        {
            _sut = new CashRegisterLogsService(_repository.Object);
        }

        [Fact]
        public async Task CreateCashRegisterLogsAsync_NoEmployee_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateCashRegisterLogsAsync(new CreateCashRegisterLogsDto()));
        }

        [Fact]
        public async Task CreateCashRegisterLogsAsync_NegativePoints_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateCashRegisterLogsAsync(new CreateCashRegisterLogsDto { EmployeeId = Guid.NewGuid(), PointsUsed = -1 }));
        }

        [Fact]
        public async Task CreateCashRegisterLogsAsync_ValidLog_AssignsNewReceiptIdAndReturnsIt()
        {
            CashRegisterLogs? saved = null;
            _repository.Setup(r => r.GetNewReceiptIdAsync()).ReturnsAsync("9000000042");
            _repository.Setup(r => r.AddAsync(It.IsAny<CashRegisterLogs>())).Callback<CashRegisterLogs>(l => saved = l);

            var result = await _sut.CreateCashRegisterLogsAsync(new CreateCashRegisterLogsDto { EmployeeId = Guid.NewGuid(), PointsEarned = 3 });

            Assert.Equal("9000000042", result);
            Assert.Equal("9000000042", saved?.ReceiptId);
            Assert.Equal(3, saved?.PointsEarned);
        }

        [Fact]
        public async Task GetByReceiptIdAsync_InvalidFormat_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetByReceiptIdAsync("12345"));
        }

        [Fact]
        public async Task GetByReceiptIdAsync_NotFound_ThrowsKeyNotFoundException()
        {
            _repository.Setup(r => r.GetByReceiptIdAsync("9000000001", false)).ReturnsAsync((CashRegisterLogs?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByReceiptIdAsync("9000000001"));
        }

        [Fact]
        public async Task GetByReceiptIdAsync_Found_ReturnsDtoWithReturnedAmounts()
        {
            var log = new CashRegisterLogs
            {
                Id = Guid.NewGuid(),
                ReceiptId = "9000000001",
                ProductsSold = [new RegisterItemDetail { Id = Guid.NewGuid(), Product = TestData.Product(), Amount = 3, ReturnedAmount = 1, PriceAtTimeOfSale = 4m }],
            };
            _repository.Setup(r => r.GetByReceiptIdAsync("9000000001", false)).ReturnsAsync(log);

            var result = await _sut.GetByReceiptIdAsync("9000000001");

            Assert.Equal("9000000001", result.ReceiptId);
            Assert.Equal(1, result.ProductsSold.Single().ReturnedAmount);
        }

        [Fact]
        public async Task GetAllCashRegisterLogsAsync_ValidPagination_ReturnsLogs()
        {
            _repository.Setup(r => r.GetAllCashRegisterLogsAsync(It.IsAny<CashRegisterLogsPaginationDto>()))
                .ReturnsAsync(new[] { new CashRegisterLogs { Id = Guid.NewGuid(), ReceiptId = "9000000001" } });

            var result = await _sut.GetAllCashRegisterLogsAsync(new CashRegisterLogsPaginationDto { PageNumber = 1, PageSize = 10 });

            Assert.Single(result);
        }
    }

    public class CustomerLogsServiceTests
    {
        private readonly Mock<ICustomerLogsRepository> _repository = new();
        private readonly CustomerLogsService _sut;

        public CustomerLogsServiceTests()
        {
            _sut = new CustomerLogsService(_repository.Object);
        }

        [Fact]
        public async Task CreateCustomerLogsAsync_NoCustomer_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateCustomerLogsAsync(new CreateCustomerLogsDto { LogType = LogTypeDto.Update }));
        }

        [Fact]
        public async Task CreateCustomerLogsAsync_InvalidLogType_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateCustomerLogsAsync(new CreateCustomerLogsDto { CustomerId = Guid.NewGuid(), LogType = (LogTypeDto)99 }));
        }

        [Fact]
        public async Task CreateCustomerLogsAsync_ValidLog_AddsEntityWithMessages()
        {
            CustomerLogs? saved = null;
            _repository.Setup(r => r.AddAsync(It.IsAny<CustomerLogs>())).Callback<CustomerLogs>(l => saved = l);

            await _sut.CreateCustomerLogsAsync(new CreateCustomerLogsDto { CustomerId = Guid.NewGuid(), LogType = LogTypeDto.Update, Messages = ["Password: Changed."] });

            Assert.Equal(LogType.Update, saved?.LogType);
            Assert.Contains("Password: Changed.", saved!.Messages);
        }

        [Fact]
        public async Task GetAllCustomerLogsAsync_ValidPagination_ReturnsLogs()
        {
            var log = new CustomerLogs { Id = Guid.NewGuid(), Customer = TestData.Customer(), LogType = LogType.Login };
            _repository.Setup(r => r.GetAllCustomerLogsAsync(It.IsAny<CustomerLogsPaginationDto>())).ReturnsAsync(new[] { log });

            var result = await _sut.GetAllCustomerLogsAsync(new CustomerLogsPaginationDto { PageNumber = 1, PageSize = 10 });

            Assert.Equal(LogTypeDto.Login, result.Single().LogType);
        }
    }

    public class EmployeeLogsServiceTests
    {
        private readonly Mock<IEmployeeLogsRepository> _repository = new();
        private readonly EmployeeLogsService _sut;

        public EmployeeLogsServiceTests()
        {
            _sut = new EmployeeLogsService(_repository.Object);
        }

        [Fact]
        public async Task CreateEmployeeLogsAsync_NoIdentifier_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateEmployeeLogsAsync(new CreateEmployeeLogsDto { EmployeeId = Guid.NewGuid() }));
        }

        [Fact]
        public async Task CreateEmployeeLogsAsync_ValidLog_AddsEntity()
        {
            await _sut.CreateEmployeeLogsAsync(new CreateEmployeeLogsDto { EmployeeId = Guid.NewGuid(), Identifier = "000010", LogType = LogTypeDto.Login });

            _repository.Verify(r => r.AddAsync(It.Is<EmployeeLogs>(l => l.Identifier == "000010" && l.LogType == LogType.Login)), Times.Once);
        }

        [Fact]
        public async Task GetAllEmployeeLogsAsync_ValidPagination_ReturnsLogs()
        {
            var log = new EmployeeLogs { Id = Guid.NewGuid(), Identifier = "000010", Employee = TestData.Employee() };
            _repository.Setup(r => r.GetAllEmployeeLogsAsync(It.IsAny<EmployeeLogsPaginationDto>())).ReturnsAsync(new[] { log });

            var result = await _sut.GetAllEmployeeLogsAsync(new EmployeeLogsPaginationDto { PageNumber = 1, PageSize = 10 });

            Assert.Equal("000010", result.Single().Identifier);
        }
    }

    public class StockLogsServiceTests
    {
        private readonly Mock<IStockLogsRepository> _repository = new();
        private readonly Mock<IProductRepository> _productRepository = new();
        private readonly StockLogsService _sut;

        public StockLogsServiceTests()
        {
            _sut = new StockLogsService(_repository.Object, _productRepository.Object);
        }

        [Fact]
        public async Task CreateStockLogsAsync_NoEmployee_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateStockLogsAsync(new CreateStockLogsDto { Identifier = "500000001" }));
        }

        [Fact]
        public async Task CreateStockLogsAsync_ValidLog_AddsEntity()
        {
            await _sut.CreateStockLogsAsync(new CreateStockLogsDto { Identifier = "500000001", EmployeeId = Guid.NewGuid(), LogType = LogTypeDto.Returned, StockLogType = StockLogTypeDto.Product });

            _repository.Verify(r => r.AddAsync(It.Is<StockLogs>(l => l.LogType == LogType.Returned && l.StockLogType == StockLogType.Product)), Times.Once);
        }

        [Fact]
        public async Task GetProductStockHistoryAsync_InvalidBarcode_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetProductStockHistoryAsync(1234));
        }

        [Fact]
        public async Task GetProductStockHistoryAsync_ProductNotFound_ThrowsKeyNotFoundException()
        {
            _productRepository.Setup(r => r.GetProductByBarcodeAsync(500000001)).ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetProductStockHistoryAsync(500000001));
        }

        [Fact]
        public async Task GetProductStockHistoryAsync_ProductFound_ReturnsProductAndLogs()
        {
            var product = TestData.Product();
            var log = new StockLogs { Id = Guid.NewGuid(), Identifier = "500000001", Employee = TestData.Employee(), LogType = LogType.Sold, Messages = ["Stock: New: 9. Old: 10"] };
            _productRepository.Setup(r => r.GetProductByBarcodeAsync(500000001)).ReturnsAsync(product);
            _repository.Setup(r => r.GetLogsByIdentifierAsync(StockLogType.Product, "500000001")).ReturnsAsync(new[] { log });

            var result = await _sut.GetProductStockHistoryAsync(500000001);

            Assert.Equal(product.Id, result.Product.Id);
            Assert.Equal("Bosch", result.Product.Brand?.BrandName);
            Assert.Single(result.Logs);
        }

        [Fact]
        public async Task GetAllStockLogsAsync_ValidPagination_ReturnsLogs()
        {
            var log = new StockLogs { Id = Guid.NewGuid(), Identifier = "Bosch", Employee = TestData.Employee() };
            _repository.Setup(r => r.GetAllStockLogsAsync(It.IsAny<StockLogsPaginationDto>())).ReturnsAsync(new[] { log });

            var result = await _sut.GetAllStockLogsAsync(new StockLogsPaginationDto { PageNumber = 1, PageSize = 10 });

            Assert.Single(result);
        }
    }
}
