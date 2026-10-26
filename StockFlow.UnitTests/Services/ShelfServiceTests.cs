using StockFlow.Application.Services.ManagementApp.Stock;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services
{
    public class ShelfServiceTests
    {
        private readonly Mock<IShelfRepository> _shelfRepository = new();
        private readonly Mock<IStockLogsService> _stockLogsService = new();
        private readonly ShelfService _sut;

        public ShelfServiceTests()
        {
            _sut = new ShelfService(_shelfRepository.Object, _stockLogsService.Object);
        }

        [Fact]
        public async Task CreateShelfAsync_MissingLocation_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateShelfAsync(new CreateShelfDto { ShelfName = "Paint" }, Guid.NewGuid()));
        }

        [Fact]
        public async Task CreateShelfAsync_ValidShelf_AddsAndLogs()
        {
            Shelf? created = null;
            _shelfRepository.Setup(r => r.AddAsync(It.IsAny<Shelf>())).Callback<Shelf>(s => created = s);

            await _sut.CreateShelfAsync(new CreateShelfDto { ShelfName = "Paint", ShelfLocation = "004", ShelfHallway = "B" }, Guid.NewGuid());

            Assert.Equal("Paint", created?.ShelfName);
            Assert.Equal("004", created?.ShelfLocation);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.StockLogType == StockLogTypeDto.Shelf && l.Identifier == "Paint")), Times.Once);
        }

        [Fact]
        public async Task GetAllShelvesAsync_ValidPagination_ReturnsShelves()
        {
            _shelfRepository.Setup(r => r.GetAllShelvesAsync(It.IsAny<ShelfPaginationDto>())).ReturnsAsync(new[] { TestData.Shelf() });

            var result = await _sut.GetAllShelvesAsync(new ShelfPaginationDto { PageNumber = 1, PageSize = 10 });

            Assert.Single(result);
        }

        [Fact]
        public async Task UpdateShelfAsync_ShelfNotFound_ThrowsKeyNotFoundException()
        {
            var dto = new UpdateShelfDto { Id = Guid.NewGuid(), ShelfName = "Paint", ShelfLocation = "004" };
            _shelfRepository.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Shelf?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateShelfAsync(dto, Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateShelfAsync_ShelfFound_UpdatesAndLogs()
        {
            var shelf = TestData.Shelf();
            _shelfRepository.Setup(r => r.GetByIdAsync(shelf.Id)).ReturnsAsync(shelf);

            await _sut.UpdateShelfAsync(new UpdateShelfDto { Id = shelf.Id, ShelfName = "Paint", ShelfLocation = "009" }, Guid.NewGuid());

            Assert.Equal("Paint", shelf.ShelfName);
            Assert.Equal("009", shelf.ShelfLocation);
            _shelfRepository.Verify(r => r.UpdateAsync(shelf), Times.Once);
            _stockLogsService.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.LogType == LogTypeDto.Update)), Times.Once);
        }

        [Fact]
        public async Task SoftDeleteShelfAsync_ShelfNotFound_ThrowsKeyNotFoundException()
        {
            _shelfRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Shelf?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.SoftDeleteShelfAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task SoftDeleteShelfAsync_ShelfFound_SoftDeletesAndLogs()
        {
            var shelf = TestData.Shelf();
            _shelfRepository.Setup(r => r.GetByIdAsync(shelf.Id)).ReturnsAsync(shelf);

            await _sut.SoftDeleteShelfAsync(shelf.Id, Guid.NewGuid());

            _shelfRepository.Verify(r => r.SoftDeleteAsync(shelf), Times.Once);
        }
    }
}
