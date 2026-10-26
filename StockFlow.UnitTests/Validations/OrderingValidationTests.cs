using StockFlow.Application.Validations.Stock;
using StockFlow.Shared.DTOs.Ordering;
using StockFlow.Shared.DTOs.Stock.Supplier;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Validations
{
    public class OrderingValidationTests
    {
        private static readonly DateTime Today = DateTime.Today;

        [Fact]
        public void Validate_ProductMaximumBelowMinimum_ThrowsArgumentException()
        {
            var dto = TestData.CreateProductDto(Guid.NewGuid(), Guid.NewGuid());
            dto.MinimumStock = 10;
            dto.MaximumStock = 5;

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void Validate_ProductAutoOrderWithoutMinimum_ThrowsArgumentException()
        {
            var dto = TestData.CreateProductDto(Guid.NewGuid(), Guid.NewGuid());
            dto.AutoOrder = true;

            var exception = Assert.Throws<ArgumentException>(() => dto.Validate());
            Assert.Equal("Set a minimum stock to order automatically.", exception.Message);
        }

        [Fact]
        public void Validate_ProductAutoOrderButNotOrderable_ThrowsArgumentException()
        {
            var dto = TestData.CreateProductDto(Guid.NewGuid(), Guid.NewGuid());
            dto.MinimumStock = 5;
            dto.AutoOrder = true;
            dto.Orderable = false;

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void Validate_ProductWithoutSupplier_ThrowsArgumentException()
        {
            var dto = TestData.CreateProductDto(Guid.NewGuid(), Guid.NewGuid());
            dto.SupplierId = Guid.Empty;

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void Validate_ValidStockSettings_DoesNotThrow()
        {
            var dto = TestData.CreateProductDto(Guid.NewGuid(), Guid.NewGuid());
            dto.MinimumStock = 5;
            dto.MaximumStock = 20;
            dto.AutoOrder = true;
            dto.Status = ProductStatusDto.Draft;

            Assert.Null(Record.Exception(() => dto.Validate()));
        }

        [Theory]
        [InlineData(-1, 9.99)]
        [InlineData(0, 9.999)]
        [InlineData(0, -1)]
        public void Validate_SchedulePriceInvalid_ThrowsArgumentException(int dayOffset, double price)
        {
            var request = new SchedulePriceRequest { ProductId = Guid.NewGuid(), Price = (decimal)price, ValidFrom = Today.AddDays(dayOffset) };

            Assert.Throws<ArgumentException>(() => request.Validate(Today));
        }

        [Fact]
        public void Validate_SchedulePriceToday_DoesNotThrow()
        {
            var request = new SchedulePriceRequest { ProductId = Guid.NewGuid(), Price = 9.99m, ValidFrom = Today };

            Assert.Null(Record.Exception(() => request.Validate(Today)));
        }

        [Fact]
        public void Validate_RecallWithoutReason_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new RecallRequest { ProductId = Guid.NewGuid(), Reason = " " }.Validate());
        }

        [Fact]
        public void Validate_OrderBlockEndingInThePast_ThrowsArgumentException()
        {
            var request = new OrderBlockRequest { ProductId = Guid.NewGuid(), Reason = "Supplier issue", Until = Today.AddDays(-1) };

            Assert.Throws<ArgumentException>(() => request.Validate(Today));
        }

        [Fact]
        public void Validate_SupplierWithBadEmail_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CreateSupplierDto { SupplierName = "Paint Co", Email = "nope" }.Validate());
        }

        [Fact]
        public void Validate_SupplierLeadTimeTooLong_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CreateSupplierDto { SupplierName = "Paint Co", LeadTimeDays = FieldLimits.MaxLeadTimeDays + 1 }.Validate());
        }

        [Fact]
        public void Validate_ValidSupplier_DoesNotThrow()
        {
            var dto = new UpdateSupplierDto { Id = Guid.NewGuid(), SupplierName = "Paint Co", Email = "orders@paint.example", OrderDay = DayOfWeek.Friday, LeadTimeDays = 3 };

            Assert.Null(Record.Exception(() => dto.Validate()));
        }

        [Theory]
        [InlineData(12345, 1)]
        [InlineData(500000001, 0)]
        [InlineData(500000001, FieldLimits.MaxOrderAmount + 1)]
        public void Validate_AddOrderLineInvalid_ThrowsArgumentException(int barcode, int amount)
        {
            Assert.Throws<ArgumentException>(() => new AddOrderLineRequest { Barcode = barcode, Amount = amount }.Validate());
        }

        [Fact]
        public void Validate_ReceiveSameLineTwice_ThrowsArgumentException()
        {
            var line = Guid.NewGuid();
            var request = new ReceiveOrderRequest { Lines = [new() { LineId = line, ReceivedAmount = 1 }, new() { LineId = line, ReceivedAmount = 2 }] };

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Fact]
        public void Validate_CustomerOrderWithBadCustomerId_ThrowsArgumentException()
        {
            var request = new CreateCustomerOrderRequest { CustomerId = "1234", Lines = [new() { Barcode = 500000001, Amount = 1 }] };

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Fact]
        public void Validate_CustomerOrderWithoutLines_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CreateCustomerOrderRequest { CustomerId = "80000001" }.Validate());
        }

        [Fact]
        public void Validate_StockCountNegativeCount_ThrowsArgumentException()
        {
            var request = new CompleteStockCountRequest { Lines = [new() { LineId = Guid.NewGuid(), CountedStock = -1 }] };

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Fact]
        public void Validate_BrandWithInvalidLogo_ThrowsArgumentException()
        {
            var dto = new CreateProductBrandDto { BrandName = "Bosch", Logo = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12] };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void Validate_BrandWithContactInfo_DoesNotThrow()
        {
            var dto = new CreateProductBrandDto { BrandName = "Bosch", Website = "https://bosch.example", Country = "Germany", ContactEmail = "info@bosch.example", CustomerServicePhone = "+49 711 400" };

            Assert.Null(Record.Exception(() => dto.Validate()));
        }
    }
}
