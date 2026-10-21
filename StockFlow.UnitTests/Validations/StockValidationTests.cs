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
        public void ValidateCreate_ImageIsNotAnImage_ThrowsArgumentException()
        {
            var dto = Valid();
            dto.ProductImage = "%PDF-1.7"u8.ToArray();

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateCreate_PngImage_DoesNotThrow()
        {
            var dto = Valid();
            dto.ProductImage = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];

            Assert.Null(Record.Exception(() => dto.Validate()));
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
        [Fact]
        public void ValidateCreate_SecondItemHalfPrice_NeedsNoOtherFields()
        {
            var dto = new CreateSalesDto { SaleName = "2e halve prijs", Description = "x", SaleType = SaleTypeDto.SecondItemHalfPrice, SalesTags = ["tuin"] };

            Assert.Null(Record.Exception(() => dto.Validate()));
        }

        [Fact]
        public void ValidateCreate_ValidBundle_DoesNotThrow()
        {
            var dto = new CreateSalesDto { SaleName = "3 voor 10", Description = "x", SaleType = SaleTypeDto.Bundle, BuyAmount = 3, BundlePrice = 10m, SalesTags = ["tuin"] };

            Assert.Null(Record.Exception(() => dto.Validate()));
        }

        [Theory]
        [InlineData(1, 10.0)]
        [InlineData(100, 10.0)]
        [InlineData(3, 0.0)]
        [InlineData(3, 9.999)]
        [InlineData(3, 100000.01)]
        public void ValidateCreate_InvalidBundle_ThrowsArgumentException(int size, double price)
        {
            var dto = new CreateSalesDto { SaleName = "bundel", Description = "x", SaleType = SaleTypeDto.Bundle, BuyAmount = size, BundlePrice = (decimal)price, SalesTags = ["tuin"] };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateCreate_MultiBuy_DoesNotNeedAPercentage()
        {
            var dto = new CreateSalesDto { SaleName = "1+1", Description = "x", SaleType = SaleTypeDto.MultiBuy, BuyAmount = 1, FreeAmount = 1, SalesTags = ["tuin"] };

            Assert.Null(Record.Exception(() => dto.Validate()));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(100, 1)]
        public void ValidateCreate_MultiBuyWithoutAmounts_ThrowsArgumentException(int buy, int free)
        {
            var dto = new CreateSalesDto { SaleName = "x+y", Description = "x", SaleType = SaleTypeDto.MultiBuy, BuyAmount = buy, FreeAmount = free, SalesTags = ["tuin"] };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateCreate_MovingSale_ThrowsArgumentException()
        {
            var dto = new CreateSalesDto { SaleName = "Verhuis", Description = "x", SaleType = SaleTypeDto.Moving, PercentageOff = 10, SalesTags = ["tuin"] };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateCreate_EndBeforeStart_ThrowsArgumentException()
        {
            var dto = new CreateSalesDto { SaleName = "Zomer", Description = "x", PercentageOff = 10, SalesTags = ["tuin"], StartDate = new DateTime(2026, 7, 1), EndDate = new DateTime(2026, 6, 30) };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateUpdate_MovingSaleWithoutTags_DoesNotThrow()
        {
            var dto = new UpdateSalesDto { Id = Guid.NewGuid(), SaleName = "Moving sale", Description = "x", SaleType = SaleTypeDto.Moving, PercentageOff = 12 };

            Assert.Null(Record.Exception(() => dto.Validate(isMovingSale: true)));
        }

        [Fact]
        public void ValidateUpdate_TurnIntoMovingSale_ThrowsArgumentException()
        {
            var dto = new UpdateSalesDto { Id = Guid.NewGuid(), SaleName = "Zomer", Description = "x", SaleType = SaleTypeDto.Moving, PercentageOff = 10, SalesTags = ["tuin"] };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

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
