using StockFlow.Application.Services.ManagementApp.Stock;
using StockFlow.Shared.DTOs.Stock;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services
{
    public class DeletedItemsServiceTests
    {
        private readonly Mock<IDeletedItemsRepository> _deletedItemsRepository = new();
        private readonly Mock<IProductRepository> _productRepository = new();
        private readonly Mock<IProductBrandRepository> _brandRepository = new();
        private readonly Mock<ISalesRepository> _salesRepository = new();
        private readonly Mock<IShelfRepository> _shelfRepository = new();
        private readonly Mock<IStockLogsService> _stockLogsService = new();
        private readonly DeletedItemsService _sut;

        public DeletedItemsServiceTests()
        {
            _sut = new DeletedItemsService(_deletedItemsRepository.Object, _productRepository.Object, _brandRepository.Object,
                _salesRepository.Object, _shelfRepository.Object, _stockLogsService.Object);
        }

        private Product DeletedProduct(bool brandDeleted = false, bool shelfDeleted = false)
        {
            var brand = TestData.Brand(brandDeleted);
            var shelf = TestData.Shelf(shelfDeleted);
            var product = TestData.Product(brand: brand, shelf: shelf);
            product.SoftDeleted = true;
            _productRepository.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
            _brandRepository.Setup(r => r.GetByIdAsync(brand.Id)).ReturnsAsync(brand);
            _shelfRepository.Setup(r => r.GetByIdAsync(shelf.Id)).ReturnsAsync(shelf);
            return product;
        }

        [Fact]
        public async Task GetDeletedItemsAsync_All_ReturnsEveryTypeNewestFirst()
        {
            var product = TestData.Product();
            product.SoftDeletedAt = new DateTime(2026, 10, 1);
            var brand = TestData.Brand(true);
            brand.SoftDeletedAt = new DateTime(2026, 10, 5);
            _deletedItemsRepository.Setup(r => r.GetDeletedProductsAsync()).ReturnsAsync(new[] { product });
            _deletedItemsRepository.Setup(r => r.GetDeletedBrandsAsync()).ReturnsAsync(new[] { brand });
            _deletedItemsRepository.Setup(r => r.GetDeletedSalesAsync()).ReturnsAsync(Array.Empty<Sales>());
            _deletedItemsRepository.Setup(r => r.GetDeletedShelvesAsync()).ReturnsAsync(Array.Empty<Shelf>());

            var result = await _sut.GetDeletedItemsAsync(null);

            Assert.Equal([DeletedItemTypeDto.ProductBrand, DeletedItemTypeDto.Product], result.Select(r => r.Type));
            Assert.Equal(product.Barcode.ToString(), result[1].Identifier);
        }

        [Fact]
        public async Task GetDeletedItemsAsync_OnlyShelves_AsksOnlyForShelves()
        {
            _deletedItemsRepository.Setup(r => r.GetDeletedShelvesAsync()).ReturnsAsync(new[] { TestData.Shelf(true) });

            var result = await _sut.GetDeletedItemsAsync(DeletedItemTypeDto.Shelf);

            Assert.Single(result);
            _deletedItemsRepository.Verify(r => r.GetDeletedProductsAsync(), Times.Never);
        }

        [Fact]
        public async Task RestoreAsync_DeletedProduct_RestoresAndLogs()
        {
            var product = DeletedProduct();
            var employeeId = Guid.NewGuid();

            var result = await _sut.RestoreAsync(DeletedItemTypeDto.Product, product.Id, employeeId);

            Assert.Equal(product.Barcode.ToString(), result.Identifier);
            _deletedItemsRepository.Verify(r => r.RestoreAsync(product), Times.Once);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l =>
                l.LogType == LogTypeDto.Restored && l.StockLogType == StockLogTypeDto.Product &&
                l.Identifier == product.Barcode.ToString() && l.EmployeeId == employeeId)), Times.Once);
        }

        [Fact]
        public async Task RestoreAsync_ProductWithDeletedBrand_ThrowsArgumentException()
        {
            var product = DeletedProduct(brandDeleted: true);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.RestoreAsync(DeletedItemTypeDto.Product, product.Id, Guid.NewGuid()));

            Assert.Contains("brand", exception.Message);
            _deletedItemsRepository.Verify(r => r.RestoreAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task RestoreAsync_ProductWithDeletedShelf_ThrowsArgumentException()
        {
            var product = DeletedProduct(shelfDeleted: true);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.RestoreAsync(DeletedItemTypeDto.Product, product.Id, Guid.NewGuid()));

            Assert.Contains("shelf", exception.Message);
        }

        [Fact]
        public async Task RestoreAsync_NotDeleted_ThrowsArgumentException()
        {
            var brand = TestData.Brand();
            _brandRepository.Setup(r => r.GetByIdAsync(brand.Id)).ReturnsAsync(brand);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.RestoreAsync(DeletedItemTypeDto.ProductBrand, brand.Id, Guid.NewGuid()));

            Assert.Equal(DeletedItemsService.NotDeleted, exception.Message);
        }

        [Fact]
        public async Task RestoreAsync_UnknownShelf_ThrowsKeyNotFoundException()
        {
            _shelfRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Shelf?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.RestoreAsync(DeletedItemTypeDto.Shelf, Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task RestoreAsync_DeletedSale_RestoresWithSaleLog()
        {
            var sale = TestData.Sale();
            sale.SoftDeleted = true;
            _salesRepository.Setup(r => r.GetByIdAsync(sale.Id)).ReturnsAsync(sale);

            await _sut.RestoreAsync(DeletedItemTypeDto.Sale, sale.Id, Guid.NewGuid());

            _deletedItemsRepository.Verify(r => r.RestoreAsync(sale), Times.Once);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.StockLogType == StockLogTypeDto.Sales && l.Identifier == sale.SaleName)), Times.Once);
        }
    }
}
