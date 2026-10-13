namespace StockFlow.UnitTests.Common
{
    public class AppVersionTests
    {
        [Theory]
        [InlineData("1.2.0", "v1.2.0")]
        [InlineData("v1.2.0", "v1.2.0")]
        [InlineData("1.3.0-beta.1", "v1.3.0-beta.1")]
        [InlineData("0.0.0-dev", "dev build")]
        [InlineData(null, "dev build")]
        public void Display_Version_ReturnsReadableText(string? version, string expected)
        {
            Assert.Equal(expected, AppVersion.Display(version));
        }

        [Theory]
        [InlineData("1.3.0", "1.2.9", true)]
        [InlineData("2.0.0", "1.10.0", true)]
        [InlineData("1.10.0", "1.9.0", true)]
        [InlineData("1.2.0", "1.2.0", false)]
        [InlineData("1.1.0", "1.2.0", false)]
        [InlineData("v1.3.0", "1.2.0", true)]
        [InlineData("1.3.0", "0.0.0-dev", false)]
        [InlineData("garbage", "1.2.0", false)]
        public void IsNewer_Versions_ComparesNumerically(string candidate, string current, bool expected)
        {
            Assert.Equal(expected, AppVersion.IsNewer(candidate, current));
        }

        [Theory]
        [InlineData("1.2.0", "v1.2.0", true)]
        [InlineData("1.2.0", "1.2.1", false)]
        [InlineData("1.2.0-beta", "1.2.0", false)]
        public void AreSame_Versions_ComparesCoreAndSuffix(string first, string second, bool expected)
        {
            Assert.Equal(expected, AppVersion.AreSame(first, second));
        }

        [Fact]
        public void Of_Assembly_StripsBuildMetadata()
        {
            var version = AppVersion.Of(typeof(AppVersion).Assembly);

            Assert.DoesNotContain("+", version);
            Assert.False(string.IsNullOrWhiteSpace(version));
        }
    }
}
