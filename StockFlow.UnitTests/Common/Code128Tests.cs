namespace StockFlow.UnitTests.Common
{
    public class Code128Tests
    {
        [Fact]
        public void Codes_NineDigitBarcode_UsesSetCAndSwitchesToBForTheLastDigit()
        {
            var result = Code128.Codes("500000001");

            Assert.Equal([105, 50, 0, 0, 0, 100, 17, 36], result);
        }

        [Fact]
        public void Codes_TenDigitReceiptId_UsesOnlySetC()
        {
            var result = Code128.Codes("9000000005");

            Assert.Equal(105, result[0]);
            Assert.Equal([90, 0, 0, 0, 5], result.Skip(1).Take(5));
            Assert.Equal(7, result.Count);
        }

        [Fact]
        public void Codes_Text_UsesSetB()
        {
            var result = Code128.Codes("AB");

            Assert.Equal([104, 33, 34, (104 + 33 + 34 * 2) % 103], result);
        }

        [Fact]
        public void Modules_EndsWithTheStopPattern()
        {
            var result = Code128.Modules("9000000005");

            Assert.EndsWith("1100011101011", result);
            Assert.Equal(7 * 11 + 13, result.Length);
        }

        [Theory]
        [InlineData("")]
        [InlineData("caf\u00e9")]
        public void Codes_InvalidValue_ThrowsArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(() => Code128.Codes(value));
        }

        [Fact]
        public void ToSvg_ReturnsAnSvgWithBarsAndLabel()
        {
            var svg = Code128.ToSvg("500000001", 2, 56);

            Assert.StartsWith("<svg", svg);
            Assert.Contains("aria-label=\"Barcode 500000001\"", svg);
            Assert.Contains("height=\"56\"", svg);
            Assert.EndsWith("</svg>", svg);
        }
    }
}
