using StockFlow.Shared.Common;

namespace StockFlow.UnitTests.Common
{
    public class IdentifierFormatsTests
    {
        [Theory]
        [InlineData("500000001", true)]
        [InlineData("50000001", false)]
        [InlineData("5000000011", false)]
        [InlineData("50000000a", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsProductBarcode_Input_ReturnsExpected(string? input, bool expected)
        {
            Assert.Equal(expected, IdentifierFormats.IsProductBarcode(input));
        }

        [Theory]
        [InlineData("80000001", true)]
        [InlineData("70000001", false)]
        [InlineData("8000001", false)]
        [InlineData("8000000a", false)]
        [InlineData(null, false)]
        public void IsCustomerId_Input_ReturnsExpected(string? input, bool expected)
        {
            Assert.Equal(expected, IdentifierFormats.IsCustomerId(input));
        }

        [Theory]
        [InlineData("9000000001", true)]
        [InlineData("8000000001", false)]
        [InlineData("900000001", false)]
        [InlineData("900000000a", false)]
        [InlineData(null, false)]
        public void IsReceiptId_Input_ReturnsExpected(string? input, bool expected)
        {
            Assert.Equal(expected, IdentifierFormats.IsReceiptId(input));
        }
    }
}
