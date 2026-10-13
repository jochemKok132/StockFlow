using StockFlow.Application.Services.ManagementApp.Stock;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services
{
    public class ProductBrandServiceTests
    {
        private readonly Mock<IProductBrandRepository> _brandRepository = new();
        private readonly Mock<IStockLogsService> _stockLogsService = new();
        private readonly ProductBrandService _sut;

        public ProductBrandServiceTests()
        {
            _sut = new ProductBrandService(_brandRepository.Object, _stockLogsService.Object);
        }

        [Fact]
        public async Task CreateProductBrandAsync_EmptyName_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateProductBrandAsync(new CreateProductBrandDto { BrandName = " " }, Guid.NewGuid()));
            _brandRepository.Verify(r => r.AddAsync(It.IsAny<ProductBrand>()), Times.Never);
        }

        [Fact]
        public async Task CreateProductBrandAsync_ValidBrand_AddsAndLogs()
        {
            ProductBrand? created = null;
            _brandRepository.Setup(r => r.AddAsync(It.IsAny<ProductBrand>())).Callback<ProductBrand>(b => created = b);

            await _sut.CreateProductBrandAsync(new CreateProductBrandDto { BrandName = "Makita" }, Guid.NewGuid());

            Assert.Equal("Makita", created?.BrandName);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.LogType == LogTypeDto.Create && l.StockLogType == StockLogTypeDto.ProductBrand)), Times.Once);
        }

        [Fact]
        public async Task GetAllProductBrandsAsync_ValidPagination_ReturnsBrands()
        {
            _brandRepository.Setup(r => r.GetAllProductBrandsAsync(It.IsAny<ProductBrandPaginationDto>())).ReturnsAsync(new[] { TestData.Brand(), TestData.Brand() });

            var result = await _sut.GetAllProductBrandsAsync(new ProductBrandPaginationDto { PageNumber = 1, PageSize = 10 });

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task UpdateProductBrandAsync_BrandNotFound_ThrowsKeyNotFoundException()
        {
            var dto = new UpdateProductBrandDto { Id = Guid.NewGuid(), BrandName = "Makita" };
            _brandRepository.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((ProductBrand?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateProductBrandAsync(dto, Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateProductBrandAsync_NameChanged_UpdatesAndLogsDifference()
        {
            var brand = TestData.Brand();
            CreateStockLogsDto? log = null;
            _brandRepository.Setup(r => r.GetByIdAsync(brand.Id)).ReturnsAsync(brand);
            _stockLogsService.Setup(s => s.CreateStockLogsAsync(It.IsAny<CreateStockLogsDto>())).Callback<CreateStockLogsDto>(l => log = l);

            await _sut.UpdateProductBrandAsync(new UpdateProductBrandDto { Id = brand.Id, BrandName = "Makita" }, Guid.NewGuid());

            Assert.Equal("Makita", brand.BrandName);
            _brandRepository.Verify(r => r.UpdateAsync(brand), Times.Once);
            Assert.NotNull(log);
            Assert.Contains("BrandName: New: Makita. Old: Bosch", log.Messages);
        }

        [Fact]
        public async Task SoftDeleteProductBrandAsync_BrandNotFound_ThrowsKeyNotFoundException()
        {
            _brandRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ProductBrand?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.SoftDeleteProductBrandAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task SoftDeleteProductBrandAsync_BrandFound_SoftDeletesAndLogs()
        {
            var brand = TestData.Brand();
            _brandRepository.Setup(r => r.GetByIdAsync(brand.Id)).ReturnsAsync(brand);

            await _sut.SoftDeleteProductBrandAsync(brand.Id, Guid.NewGuid());

            _brandRepository.Verify(r => r.SoftDeleteAsync(brand), Times.Once);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.LogType == LogTypeDto.Delete)), Times.Once);
        }
    }
}
