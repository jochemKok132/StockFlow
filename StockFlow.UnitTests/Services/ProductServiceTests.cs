using StockFlow.Application.Services.ManagementApp.Stock;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepository = new();
        private readonly Mock<ISalesRepository> _salesRepository = new();
        private readonly Mock<IShelfRepository> _shelfRepository = new();
        private readonly Mock<IProductBrandRepository> _brandRepository = new();
        private readonly Mock<IStockLogsService> _stockLogsService = new();
        private readonly ProductService _sut;

        public ProductServiceTests()
        {
            _sut = new ProductService(_productRepository.Object, _salesRepository.Object, _shelfRepository.Object, _brandRepository.Object, _stockLogsService.Object);
        }

        private (ProductBrand brand, Shelf shelf) SetupBrandAndShelf(bool brandDeleted = false, bool shelfDeleted = false)
        {
            var brand = TestData.Brand(brandDeleted);
            var shelf = TestData.Shelf(shelfDeleted);
            _brandRepository.Setup(r => r.GetByIdAsync(brand.Id)).ReturnsAsync(brand);
            _shelfRepository.Setup(r => r.GetByIdAsync(shelf.Id)).ReturnsAsync(shelf);
            return (brand, shelf);
        }

        [Fact]
        public async Task CreateProductAsync_MissingName_ThrowsArgumentException()
        {
            var dto = TestData.CreateProductDto(Guid.NewGuid(), Guid.NewGuid());
            dto.ProductName = "";

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateProductAsync(dto, Guid.NewGuid()));
        }

        [Fact]
        public async Task CreateProductAsync_BrandDoesNotExist_ThrowsArgumentException()
        {
            var (_, shelf) = SetupBrandAndShelf();
            var dto = TestData.CreateProductDto(Guid.NewGuid(), shelf.Id);
            _brandRepository.Setup(r => r.GetByIdAsync(dto.BrandId)).ReturnsAsync((ProductBrand?)null);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateProductAsync(dto, Guid.NewGuid()));
            _productRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task CreateProductAsync_ShelfSoftDeleted_ThrowsArgumentException()
        {
            var (brand, shelf) = SetupBrandAndShelf(shelfDeleted: true);
            var dto = TestData.CreateProductDto(brand.Id, shelf.Id);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateProductAsync(dto, Guid.NewGuid()));
        }

        [Fact]
        public async Task CreateProductAsync_ValidProduct_AssignsBarcodeAddsMatchingSalesAndLogs()
        {
            var (brand, shelf) = SetupBrandAndShelf();
            var sale = TestData.Sale(20, "gereedschap");
            Product? created = null;
            _salesRepository.Setup(r => r.GetAllSalesForProductTags(It.IsAny<List<string>>())).ReturnsAsync(new[] { sale });
            _productRepository.Setup(r => r.GetNewBarcode()).ReturnsAsync(500000123);
            _productRepository.Setup(r => r.AddAsync(It.IsAny<Product>())).Callback<Product>(p => created = p);

            await _sut.CreateProductAsync(TestData.CreateProductDto(brand.Id, shelf.Id), Guid.NewGuid());

            Assert.NotNull(created);
            Assert.Equal(500000123, created.Barcode);
            Assert.Contains(sale, created.Sales);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.LogType == LogTypeDto.Create && l.StockLogType == StockLogTypeDto.Product)), Times.Once);
        }

        [Fact]
        public async Task GetAllProductsAsync_ValidPagination_ReturnsBulkView()
        {
            _productRepository.Setup(r => r.GetAllProductsAsync(It.IsAny<ProductPaginationDto>())).ReturnsAsync(new[] { TestData.Product() });

            var result = await _sut.GetAllProductsAsync(TestData.ProductPagination());

            Assert.Single(result);
            Assert.Equal("Bosch", result[0].BrandName);
        }

        [Fact]
        public async Task GetAllProductsAsync_PageNumberZero_ThrowsArgumentException()
        {
            var pagination = TestData.ProductPagination();
            pagination.PageNumber = 0;

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetAllProductsAsync(pagination));
        }

        [Fact]
        public async Task SoftDeleteProductAsync_ProductNotFound_ThrowsKeyNotFoundException()
        {
            _productRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.SoftDeleteProductAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task SoftDeleteProductAsync_ProductFound_SoftDeletesAndLogs()
        {
            var product = TestData.Product();
            _productRepository.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

            await _sut.SoftDeleteProductAsync(product.Id, Guid.NewGuid());

            _productRepository.Verify(r => r.SoftDeleteAsync(product), Times.Once);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.LogType == LogTypeDto.Delete)), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ProductNotFound_ThrowsKeyNotFoundException()
        {
            var (brand, shelf) = SetupBrandAndShelf();
            var product = TestData.Product(brand: brand, shelf: shelf);
            _productRepository.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateProductAsync(TestData.UpdateProductDto(product), Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateProductAsync_TagsChanged_ResyncsSales()
        {
            var (brand, shelf) = SetupBrandAndShelf();
            var product = TestData.Product(brand: brand, shelf: shelf);
            var oldSale = TestData.Sale(10, "gereedschap");
            var newSale = TestData.Sale(30, "tuin");
            product.Sales.Add(oldSale);
            _productRepository.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
            _salesRepository.Setup(r => r.GetAllSalesForProductTags(It.IsAny<List<string>>())).ReturnsAsync(new[] { newSale });

            await _sut.UpdateProductAsync(TestData.UpdateProductDto(product, "tuin"), Guid.NewGuid());

            Assert.Equal("Hamer XL", product.ProductName);
            Assert.DoesNotContain(oldSale, product.Sales);
            Assert.Contains(newSale, product.Sales);
            _productRepository.Verify(r => r.UpdateAsync(product), Times.Once);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.LogType == LogTypeDto.Update)), Times.Once);
        }

        [Fact]
        public async Task GetProductDetailsAsync_ProductNotFound_ThrowsKeyNotFoundException()
        {
            _productRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetProductDetailsAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetProductDetailsAsync_ProductFound_FillsBrandAndShelf()
        {
            var (brand, shelf) = SetupBrandAndShelf();
            var product = TestData.Product(brand: brand, shelf: shelf);
            _productRepository.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

            var result = await _sut.GetProductDetailsAsync(product.Id);

            Assert.Equal("Bosch", result.Brand?.BrandName);
            Assert.Equal("Gereedschap", result.Shelf?.ShelfName);
        }

        [Fact]
        public async Task GetProductByBarcodeAsync_ProductNotFound_ThrowsKeyNotFoundException()
        {
            _productRepository.Setup(r => r.GetProductByBarcodeAsync(500000999)).ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetProductByBarcodeAsync(500000999));
        }

        [Fact]
        public async Task GetProductByBarcodeAsync_ProductFound_ReturnsProduct()
        {
            var (brand, shelf) = SetupBrandAndShelf();
            var product = TestData.Product(brand: brand, shelf: shelf);
            _productRepository.Setup(r => r.GetProductByBarcodeAsync(product.Barcode)).ReturnsAsync(product);

            var result = await _sut.GetProductByBarcodeAsync(product.Barcode);

            Assert.Equal(product.Id, result.Id);
            Assert.NotNull(result.Brand);
        }

        [Fact]
        public async Task UpdateProductStockAsync_ZeroChange_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateProductStockAsync(Guid.NewGuid(), 0, Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateProductStockAsync_ProductNotFound_ThrowsKeyNotFoundException()
        {
            _productRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateProductStockAsync(Guid.NewGuid(), -1, Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateProductStockAsync_Sale_LowersStockAndLogsSold()
        {
            var product = TestData.Product(stock: 10);
            _productRepository.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

            await _sut.UpdateProductStockAsync(product.Id, -3, Guid.NewGuid());

            Assert.Equal(7, product.Stock);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.LogType == LogTypeDto.Sold && l.Messages.Contains("Stock: New: 7. Old: 10"))), Times.Once);
        }

        [Fact]
        public async Task UpdateProductStockAsync_MoreThanInStock_AllowsNegativeStock()
        {
            var product = TestData.Product(stock: 1);
            _productRepository.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

            await _sut.UpdateProductStockAsync(product.Id, -3, Guid.NewGuid());

            Assert.Equal(-2, product.Stock);
        }

        [Fact]
        public async Task UpdateProductStockAsync_Return_RaisesStockAndLogsReturned()
        {
            var product = TestData.Product(stock: 10);
            _productRepository.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

            await _sut.UpdateProductStockAsync(product.Id, 2, Guid.NewGuid(), LogTypeDto.Returned);

            Assert.Equal(12, product.Stock);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.LogType == LogTypeDto.Returned)), Times.Once);
        }
    }
}
