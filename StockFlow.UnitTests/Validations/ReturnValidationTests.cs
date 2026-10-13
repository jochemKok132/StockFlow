using StockFlow.Application.Validations.CashRegister;
using StockFlow.Shared.DTOs.CashRegister;

namespace StockFlow.UnitTests.Validations
{
    public class ReturnValidationTests
    {
        [Fact]
        public void Validate_InvalidReceiptId_ThrowsArgumentException()
        {
            var request = new ReturnRequest
            {
                ReceiptId = "12345",
                Items = [new ReturnItem { ProductId = Guid.NewGuid(), Amount = 1 }],
            };

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Fact]
        public void Validate_NoItems_ThrowsArgumentException()
        {
            var request = new ReturnRequest { ReceiptId = "9000000001" };

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Fact]
        public void Validate_ZeroAmount_ThrowsArgumentException()
        {
            var request = new ReturnRequest
            {
                ReceiptId = "9000000001",
                Items = [new ReturnItem { ProductId = Guid.NewGuid(), Amount = 0 }],
            };

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Fact]
        public void Validate_ValidRequest_DoesNotThrow()
        {
            var request = new ReturnRequest
            {
                ReceiptId = "9000000001",
                Items = [new ReturnItem { ProductId = Guid.NewGuid(), Amount = 2 }],
            };

            var exception = Record.Exception(() => request.Validate());

            Assert.Null(exception);
        }
    }
}
