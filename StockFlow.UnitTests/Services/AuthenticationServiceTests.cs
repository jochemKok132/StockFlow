using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using StockFlow.Application.Services;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services
{
    public class AuthenticationServiceTests
    {
        private const string Key = "unit-test-signing-key-that-is-long-enough-for-hmac-sha256-0123456789";

        private static AuthenticationService CreateSut(string? key = Key, string? issuer = "https://jochemkok.nl/stockflow/api", string? expiry = "30")
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JWT:Key"] = key,
                    ["JWT:Issuer"] = issuer,
                    ["JWT:ExpiryMinutes"] = expiry,
                })
                .Build();

            return new AuthenticationService(configuration);
        }

        private static JsonWebToken Read(string jwt) => new JsonWebTokenHandler().ReadJsonWebToken(jwt);

        [Fact]
        public void GenerateToken_Employee_ContainsIdNameEmployeeIdAndRole()
        {
            var employee = TestData.Employee(EmployeeRole.Manager);

            var token = CreateSut().GenerateToken(employee);

            var jwt = Read(token.JWT);
            Assert.Contains(jwt.Claims, c => c.Value == employee.Id.ToString());
            Assert.Contains(jwt.Claims, c => c.Value == employee.FullName);
            Assert.Contains(jwt.Claims, c => c.Value == employee.EmployeeId);
            Assert.Contains(jwt.Claims, c => c.Value == "Manager" && c.Type.EndsWith("role"));
            Assert.Equal("https://jochemkok.nl/stockflow/api", jwt.Issuer);
        }

        [Fact]
        public void GenerateToken_Customer_HasCustomerRoleAndEmail()
        {
            var customer = TestData.Customer();

            var token = CreateSut().GenerateToken(customer);

            var jwt = Read(token.JWT);
            Assert.Contains(jwt.Claims, c => c.Value == AuthenticationService.CustomerRole && c.Type.EndsWith("role"));
            Assert.Contains(jwt.Claims, c => c.Value == customer.Email);
            Assert.Contains(jwt.Claims, c => c.Value == customer.CustomerId);
        }

        [Fact]
        public void GenerateToken_ExpiryFromConfiguration_SetsExpiresOn()
        {
            var before = DateTime.UtcNow;

            var token = CreateSut(expiry: "45").GenerateToken(TestData.Employee());

            Assert.InRange(token.ExpiresOn, before.AddMinutes(44), DateTime.UtcNow.AddMinutes(46));
        }

        [Fact]
        public void GenerateToken_MissingKey_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => CreateSut(key: null).GenerateToken(TestData.Employee()));
        }

        [Fact]
        public void GenerateToken_MissingIssuer_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => CreateSut(issuer: "").GenerateToken(TestData.Customer()));
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("0")]
        [InlineData("-5")]
        public void GenerateToken_InvalidExpiry_ThrowsInvalidOperationException(string expiry)
        {
            Assert.Throws<InvalidOperationException>(() => CreateSut(expiry: expiry).GenerateToken(TestData.Employee()));
        }
    }
}
