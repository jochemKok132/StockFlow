namespace StockFlow.UnitTests.Common
{
    public class ImageFormatsTests
    {
        public static TheoryData<byte[], string> Images => new()
        {
            { [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00], "image/png" },
            { [0xFF, 0xD8, 0xFF, 0xE0, 0x00], "image/jpeg" },
            { "GIF89a..."u8.ToArray(), "image/gif" },
            { "RIFF\0\0\0\0WEBPVP8 "u8.ToArray(), "image/webp" },
        };

        [Theory]
        [MemberData(nameof(Images))]
        public void DetectContentType_KnownImage_ReturnsContentType(byte[] bytes, string expected)
        {
            var result = ImageFormats.DetectContentType(bytes);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsImage_PdfFile_ReturnsFalse()
        {
            var result = ImageFormats.IsImage("%PDF-1.7"u8);

            Assert.False(result);
        }

        [Fact]
        public void IsImage_Empty_ReturnsFalse()
        {
            var result = ImageFormats.IsImage([]);

            Assert.False(result);
        }

        [Theory]
        [InlineData("image/png", true)]
        [InlineData("IMAGE/JPEG", true)]
        [InlineData("image/webp", true)]
        [InlineData("image/svg+xml", false)]
        [InlineData("application/pdf", false)]
        [InlineData(null, false)]
        public void IsAllowedContentType_ReturnsExpected(string? contentType, bool expected)
        {
            Assert.Equal(expected, ImageFormats.IsAllowedContentType(contentType));
        }
    }
}
