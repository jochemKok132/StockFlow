using StockFlow.Application.Validations;
using StockFlow.Application.Validations.AuditLogs;
using StockFlow.Application.Validations.Stock;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Validations
{
    public class ProductValidationTests
    {
        private static CreateProductDto Valid() => TestData.CreateProductDto(Guid.NewGuid(), Guid.NewGuid());

        [Fact]
        public void ValidateCreate_ValidProduct_DoesNotThrow()
        {
            Assert.Null(Record.Exception(() => Valid().Validate()));
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(100000.01)]
        [InlineData(1.999)]
        public void ValidateCreate_InvalidPrice_ThrowsArgumentException(double price)
        {
            var dto = Valid();
            dto.Price = (decimal)price;

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateCreate_NegativeStock_IsAllowed()
        {
            var dto = Valid();
            dto.Stock = -5;

            Assert.Null(Record.Exception(() => dto.Validate()));
        }

        [Fact]
        public void ValidateCreate_NegativeLocation_ThrowsArgumentException()
        {
            var dto = Valid();
            dto.Location = -1;

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateCreate_ImageOver5Mb_ThrowsArgumentException()
        {
            var dto = Valid();
            dto.ProductImage = new byte[5 * 1024 * 1024 + 1];

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateCreate_EmptyTag_ThrowsArgumentException()
        {
            var dto = Valid();
            dto.SalesTags = ["tuin", " "];

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateCreate_NoBrand_ThrowsArgumentException()
        {
            var dto = TestData.CreateProductDto(Guid.Empty, Guid.NewGuid());

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateStockChange_Zero_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ProductValidation.ValidateStockChange(0));
        }

        [Fact]
        public void ValidateUpdate_EmptyId_ThrowsArgumentException()
        {
            var dto = TestData.UpdateProductDto(TestData.Product());
            dto.Id = Guid.Empty;

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }
    }

    public class SalesValidationTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        public void ValidateCreate_PercentageOutOfRange_ThrowsArgumentException(int percentage)
        {
            var dto = new CreateSalesDto { SaleName = "Zomer", Description = "x", PercentageOff = percentage, SalesTags = ["tuin"] };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(99)]
        public void ValidateCreate_PercentageBoundaries_DoesNotThrow(int percentage)
        {
            var dto = new CreateSalesDto { SaleName = "Zomer", Description = "x", PercentageOff = percentage, SalesTags = ["tuin"] };

            Assert.Null(Record.Exception(() => dto.Validate()));
        }

        [Fact]
        public void ValidateUpdate_NoTags_ThrowsArgumentException()
        {
            var dto = new UpdateSalesDto { Id = Guid.NewGuid(), SaleName = "Zomer", Description = "x", PercentageOff = 10 };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }
    }

    public class ShelfAndBrandValidationTests
    {
        [Fact]
        public void ValidateShelf_HallwayTooLong_ThrowsArgumentException()
        {
            var dto = new CreateShelfDto { ShelfName = "Verf", ShelfLocation = "001", ShelfHallway = new string('A', 51) };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateShelf_NoHallway_DoesNotThrow()
        {
            Assert.Null(Record.Exception(() => new CreateShelfDto { ShelfName = "Verf", ShelfLocation = "001" }.Validate()));
        }

        [Fact]
        public void ValidateUpdateShelf_EmptyId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new UpdateShelfDto { ShelfName = "Verf", ShelfLocation = "001" }.Validate());
        }

        [Fact]
        public void ValidateBrand_NameTooLong_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CreateProductBrandDto { BrandName = new string('B', 101) }.Validate());
        }

        [Fact]
        public void ValidateUpdateBrand_EmptyId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new UpdateProductBrandDto { BrandName = "Bosch" }.Validate());
        }
    }

    public class PaginationAndLogValidationTests
    {
        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(1, 101)]
        public void ValidatePagination_OutOfRange_ThrowsArgumentException(int pageNumber, int pageSize)
        {
            var pagination = new ShelfPaginationDto { PageNumber = pageNumber, PageSize = pageSize };

            Assert.Throws<ArgumentException>(() => pagination.Validate());
        }

        [Fact]
        public void ValidatePagination_MaxPageSize_DoesNotThrow()
        {
            Assert.Null(Record.Exception(() => new ShelfPaginationDto { PageNumber = 1, PageSize = PaginationValidation.MaxPageSize }.Validate()));
        }

        [Fact]
        public void ValidatePagination_InvalidOrderType_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ShelfPaginationDto { PageNumber = 1, PageSize = 10, OrderType = (OrderType)5 }.Validate());
        }

        [Fact]
        public void ValidateStockLog_InvalidStockLogType_ThrowsArgumentException()
        {
            var log = new CreateStockLogsDto { Identifier = "x", EmployeeId = Guid.NewGuid(), StockLogType = (StockLogTypeDto)9 };

            Assert.Throws<ArgumentException>(() => log.Validate());
        }

        [Fact]
        public void ValidateCashRegisterLog_NegativePointsEarned_ThrowsArgumentException()
        {
            var log = new CreateCashRegisterLogsDto { EmployeeId = Guid.NewGuid(), PointsEarned = -1 };

            Assert.Throws<ArgumentException>(() => log.Validate());
        }
    }
}
