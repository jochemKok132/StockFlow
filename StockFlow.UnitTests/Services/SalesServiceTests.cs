using StockFlow.Application.Services.ManagementApp.Stock;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services
{
    public class SalesServiceTests
    {
        private readonly Mock<ISalesRepository> _salesRepository = new();
        private readonly Mock<IProductRepository> _productRepository = new();
        private readonly Mock<IStockLogsService> _stockLogsService = new();
        private readonly SalesService _sut;

        public SalesServiceTests()
        {
            _sut = new SalesService(_salesRepository.Object, _productRepository.Object, _stockLogsService.Object);
        }

        private static CreateSalesDto ValidCreate() => new() { SaleName = "Zomer", Description = "Zomerkorting", PercentageOff = 20, SalesTags = ["tuin"] };

        [Fact]
        public async Task CreateSalesAsync_NoTags_ThrowsArgumentException()
        {
            var dto = ValidCreate();
            dto.SalesTags = [];

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateSalesAsync(dto, Guid.NewGuid()));
        }

        [Fact]
        public async Task CreateSalesAsync_ValidSale_AddsMatchingProductsAndLogs()
        {
            var product = TestData.Product();
            Sales? created = null;
            _productRepository.Setup(r => r.GetAllProductsWithTagsAsync(It.IsAny<List<string>>())).ReturnsAsync(new[] { product });
            _salesRepository.Setup(r => r.AddAsync(It.IsAny<Sales>())).Callback<Sales>(s => created = s);

            await _sut.CreateSalesAsync(ValidCreate(), Guid.NewGuid());

            Assert.NotNull(created);
            Assert.Contains(product, created.Products);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.StockLogType == StockLogTypeDto.Sales && l.LogType == LogTypeDto.Create)), Times.Once);
        }

        [Fact]
        public async Task GetAllSalesAsync_ValidPagination_ReturnsSales()
        {
            _salesRepository.Setup(r => r.GetAllSalesAsync(It.IsAny<SalesPaginationDto>(), It.IsAny<DateTime>())).ReturnsAsync(new[] { TestData.Sale() });
            _salesRepository.Setup(r => r.GetItemsSoldAsync(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(new Dictionary<Guid, int>());

            var result = await _sut.GetAllSalesAsync(new SalesPaginationDto { PageNumber = 1, PageSize = 10 });

            Assert.Single(result);
            Assert.Equal(20, result[0].PercentageOff);
        }

        [Fact]
        public async Task UpdateSalesAsync_SaleNotFound_ThrowsKeyNotFoundException()
        {
            var dto = new UpdateSalesDto { Id = Guid.NewGuid(), SaleName = "Zomer", Description = "x", PercentageOff = 10, SalesTags = ["tuin"] };
            _salesRepository.Setup(r => r.GetByIdWithProductsAsync(dto.Id)).ReturnsAsync((Sales?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateSalesAsync(dto, Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateSalesAsync_TagsChanged_ResyncsProducts()
        {
            var sale = TestData.Sale(20, "tuin");
            var oldProduct = TestData.Product();
            var newProduct = TestData.Product();
            sale.Products.Add(oldProduct);
            _salesRepository.Setup(r => r.GetByIdWithProductsAsync(sale.Id)).ReturnsAsync(sale);
            _productRepository.Setup(r => r.GetAllProductsWithTagsAsync(It.IsAny<List<string>>())).ReturnsAsync(new[] { newProduct });
            var dto = new UpdateSalesDto { Id = sale.Id, SaleName = "Winter", Description = sale.Description, PercentageOff = 30, SalesTags = ["gereedschap"] };

            await _sut.UpdateSalesAsync(dto, Guid.NewGuid());

            Assert.Equal("Winter", sale.SaleName);
            Assert.Equal(30, sale.PercentageOff);
            Assert.DoesNotContain(oldProduct, sale.Products);
            Assert.Contains(newProduct, sale.Products);
            _salesRepository.Verify(r => r.UpdateAsync(sale), Times.Once);
        }

        [Fact]
        public async Task SoftDeleteSalesAsync_SaleNotFound_ThrowsKeyNotFoundException()
        {
            _salesRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Sales?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.SoftDeleteSalesAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task SoftDeleteSalesAsync_SaleFound_SoftDeletesAndLogs()
        {
            var sale = TestData.Sale();
            _salesRepository.Setup(r => r.GetByIdAsync(sale.Id)).ReturnsAsync(sale);

            await _sut.SoftDeleteSalesAsync(sale.Id, Guid.NewGuid());

            _salesRepository.Verify(r => r.SoftDeleteAsync(sale), Times.Once);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.LogType == LogTypeDto.Delete)), Times.Once);
        }
    }
}
