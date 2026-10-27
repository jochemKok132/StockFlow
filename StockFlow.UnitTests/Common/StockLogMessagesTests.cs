namespace StockFlow.UnitTests.Common
{
    public class StockLogMessagesTests
    {
        [Fact]
        public void TryParseStock_StockMessage_ReturnsNewAndOld()
        {
            Assert.True(StockLogMessages.TryParseStock("Stock: New: -2. Old: 3", out int newStock, out int oldStock));
            Assert.Equal(-2, newStock);
            Assert.Equal(3, oldStock);
        }

        [Fact]
        public void TryParseStock_OtherMessage_ReturnsFalse()
        {
            Assert.False(StockLogMessages.TryParseStock("Price: New: 2.00. Old: 1.00", out _, out _));
        }

        [Fact]
        public void StockChange_NoStockMessage_ReturnsNull()
        {
            Assert.Null(StockLogMessages.StockChange(["Order: 1001"]));
        }

        [Fact]
        public void ReceiptId_SoldLog_ReturnsTheReceiptId()
        {
            Assert.Equal("9000000012", StockLogMessages.ReceiptId(["Stock: New: 1. Old: 2", "Receipt: 9000000012"]));
            Assert.Null(StockLogMessages.ReceiptId(["Stock: New: 1. Old: 2"]));
        }
    }
}
