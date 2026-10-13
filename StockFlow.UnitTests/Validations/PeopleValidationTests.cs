using StockFlow.Application.Validations.People;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Validations
{
    public class CustomerValidationTests
    {
        [Fact]
        public void ValidateRegister_ValidRequest_DoesNotThrow()
        {
            var exception = Record.Exception(() => TestData.RegisterRequest().Validate());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("J")]
        [InlineData("   ")]
        public void ValidateRegister_NameTooShort_ThrowsArgumentException(string name)
        {
            var request = TestData.RegisterRequest();
            request.Name = name;

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Theory]
        [InlineData("geen-email")]
        [InlineData("")]
        public void ValidateRegister_InvalidEmail_ThrowsArgumentException(string email)
        {
            var request = TestData.RegisterRequest();
            request.Email = email;

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Theory]
        [InlineData("welkom123!")]
        [InlineData("WELKOM123!")]
        [InlineData("Welkom!!!")]
        [InlineData("Welkom123")]
        [InlineData("We1!")]
        public void ValidateRegister_WeakPassword_ThrowsArgumentException(string password)
        {
            var request = TestData.RegisterRequest();
            request.Password = password;
            request.ConfirmPassword = password;

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Theory]
        [InlineData("0123 AB")]
        [InlineData("123 AB")]
        [InlineData("1234 A")]
        [InlineData("ABCD 12")]
        public void ValidateRegister_InvalidPostalCode_ThrowsArgumentException(string postalCode)
        {
            var request = TestData.RegisterRequest();
            request.PostalCode = postalCode;

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Theory]
        [InlineData("A12")]
        [InlineData("12-1")]
        [InlineData("12ABCD")]
        public void ValidateRegister_InvalidHouseNumber_ThrowsArgumentException(string houseNumber)
        {
            var request = TestData.RegisterRequest();
            request.HouseNumber = houseNumber;

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Theory]
        [InlineData("1234ab", "1234 AB")]
        [InlineData("1234 AB", "1234 AB")]
        [InlineData("9999zz", "9999 ZZ")]
        public void NormalizePostalCode_Input_ReturnsSpacedUppercase(string input, string expected)
        {
            Assert.Equal(expected, CustomerValidation.NormalizePostalCode(input));
        }

        [Fact]
        public void ValidateUpdate_NegativePoints_ThrowsArgumentException()
        {
            var dto = new UpdateCustomerDto { Id = Guid.NewGuid(), FullName = "Jan", Email = "jan@example.com", SavedPoints = -1 };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateUpdate_EmptyPostalCode_IsAllowed()
        {
            var dto = new UpdateCustomerDto { Id = Guid.NewGuid(), FullName = "Jan", Email = "jan@example.com", PostalCode = "" };

            Assert.Null(Record.Exception(() => dto.Validate()));
        }

        [Fact]
        public void ValidateChangePassword_SameAsCurrent_ThrowsArgumentException()
        {
            var request = new ChangePasswordRequest { CurrentPassword = "Welkom123!", NewPassword = "Welkom123!", ConfirmPassword = "Welkom123!" };

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Fact]
        public void ValidateChangePassword_ConfirmationDiffers_ThrowsArgumentException()
        {
            var request = new ChangePasswordRequest { CurrentPassword = "Oud12345!", NewPassword = "Nieuw123!", ConfirmPassword = "Nieuw124!" };

            Assert.Throws<ArgumentException>(() => request.Validate());
        }

        [Fact]
        public void ValidateLogin_MissingPassword_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CustomerLogin { Email = "jan@example.com", Password = "" }.Validate());
        }
    }

    public class EmployeeValidationTests
    {
        [Fact]
        public void ValidateCreate_ValidEmployee_DoesNotThrow()
        {
            var dto = new CreateEmployeeDto { FullName = "Piet", Password = "001273", Role = EmployeeRoleDto.Cashier };

            Assert.Null(Record.Exception(() => dto.Validate()));
        }

        [Theory]
        [InlineData("123")]
        [InlineData("1234567890123")]
        [InlineData("12ab")]
        [InlineData("")]
        public void ValidateCreate_InvalidPassword_ThrowsArgumentException(string password)
        {
            var dto = new CreateEmployeeDto { FullName = "Piet", Password = password, Role = EmployeeRoleDto.Cashier };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateCreate_UnknownRole_ThrowsArgumentException()
        {
            var dto = new CreateEmployeeDto { FullName = "Piet", Password = "1234", Role = (EmployeeRoleDto)42 };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateUpdate_NoNewPassword_DoesNotThrow()
        {
            var dto = new UpdateEmployeeDto { Id = Guid.NewGuid(), FullName = "Piet", Role = EmployeeRoleDto.Manager };

            Assert.Null(Record.Exception(() => dto.Validate()));
        }

        [Fact]
        public void ValidateUpdate_InvalidNewPassword_ThrowsArgumentException()
        {
            var dto = new UpdateEmployeeDto { Id = Guid.NewGuid(), FullName = "Piet", Role = EmployeeRoleDto.Manager, NewPassword = "abc" };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Fact]
        public void ValidateUpdate_EmptyId_ThrowsArgumentException()
        {
            var dto = new UpdateEmployeeDto { FullName = "Piet", Role = EmployeeRoleDto.Manager };

            Assert.Throws<ArgumentException>(() => dto.Validate());
        }

        [Theory]
        [InlineData("0000001")]
        [InlineData("00a001")]
        [InlineData("")]
        public void ValidateLogin_InvalidEmployeeId_ThrowsArgumentException(string employeeId)
        {
            Assert.Throws<ArgumentException>(() => new EmployeeLogin { EmployeeId = employeeId, Password = "001273" }.Validate());
        }
    }
}
