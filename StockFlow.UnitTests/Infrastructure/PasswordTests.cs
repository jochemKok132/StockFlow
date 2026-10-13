using StockFlow.Infrastructure.Helpers;

namespace StockFlow.UnitTests.Infrastructure
{
    public class PasswordTests
    {
        private readonly Password _sut = new();

        [Fact]
        public void Hash_Password_IsNotPlainTextAndValidates()
        {
            var hash = _sut.Hash("001273");

            Assert.NotEqual("001273", hash);
            Assert.True(_sut.Validate(hash, "001273"));
        }

        [Fact]
        public void Validate_WrongPassword_ReturnsFalse()
        {
            var hash = _sut.Hash("001273");

            Assert.False(_sut.Validate(hash, "999999"));
        }

        [Fact]
        public void Hash_SamePasswordTwice_UsesDifferentSalt()
        {
            Assert.NotEqual(_sut.Hash("001273"), _sut.Hash("001273"));
        }
    }
}
