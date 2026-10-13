using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockFlow.API.Controllers;
using StockFlow.API.Controllers.CashRegister;

namespace StockFlow.UnitTests.Api
{
    internal static class ControllerExtensions
    {
        public static T WithUser<T>(this T controller, Guid? userId) where T : ControllerBase
        {
            var claims = userId is Guid id ? new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) } : [];
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test")) },
            };
            return controller;
        }

        public static ApiResponse<T> Body<T>(this IActionResult result) =>
            Assert.IsType<ApiResponse<T>>(Assert.IsAssignableFrom<ObjectResult>(result).Value);

        public static int? Status(this IActionResult result) => Assert.IsAssignableFrom<ObjectResult>(result).StatusCode;
    }

    public class CashRegisterControllerTests
    {
        private readonly Mock<IRegisterDataService> _registerDataService = new();
        private readonly Mock<IProductService> _productService = new();
        private readonly Mock<ICustomerService> _customerService = new();
        private readonly Mock<ICashRegisterLogsService> _cashRegisterLogsService = new();
        private readonly Guid _userId = Guid.NewGuid();

        private CashRegisterController CreateSut(bool loggedIn = true) =>
            new CashRegisterController(_registerDataService.Object, _productService.Object, _customerService.Object, _cashRegisterLogsService.Object)
                .WithUser(loggedIn ? _userId : null);

        [Fact]
        public async Task FinalizePurchaseAsync_NotLoggedIn_Returns401()
        {
            var result = await CreateSut(loggedIn: false).FinalizePurchaseAsync(new Purchase());

            Assert.Equal(StatusCodes.Status401Unauthorized, result.Status());
        }

        [Fact]
        public async Task FinalizePurchaseAsync_Success_ReturnsReceiptId()
        {
            _registerDataService.Setup(s => s.FinalizePurchase(It.IsAny<Purchase>(), _userId)).ReturnsAsync("9000000001");

            var result = await CreateSut().FinalizePurchaseAsync(new Purchase());

            Assert.Equal(StatusCodes.Status200OK, result.Status());
            Assert.Equal("9000000001", result.Body<string>().Value);
        }

        [Fact]
        public async Task FinalizePurchaseAsync_ValidationError_Returns400WithMessage()
        {
            _registerDataService.Setup(s => s.FinalizePurchase(It.IsAny<Purchase>(), _userId)).ThrowsAsync(new ArgumentException("Nope."));

            var result = await CreateSut().FinalizePurchaseAsync(new Purchase());

            Assert.Equal(StatusCodes.Status400BadRequest, result.Status());
            Assert.False(result.Body<string>().Succeeded);
            Assert.Equal("Nope.", result.Body<string>().Message);
        }

        [Fact]
        public async Task FinalizePurchaseAsync_ProductMissing_Returns404()
        {
            _registerDataService.Setup(s => s.FinalizePurchase(It.IsAny<Purchase>(), _userId)).ThrowsAsync(new KeyNotFoundException("Gone."));

            var result = await CreateSut().FinalizePurchaseAsync(new Purchase());

            Assert.Equal(StatusCodes.Status404NotFound, result.Status());
        }

        [Fact]
        public async Task ReturnAsync_Success_ReturnsResult()
        {
            var returnResult = new ReturnResult { ReceiptId = "9000000001", ItemsReturned = 1, RefundAmount = 4m };
            _registerDataService.Setup(s => s.ProcessReturn(It.IsAny<ReturnRequest>(), _userId)).ReturnsAsync(returnResult);

            var result = await CreateSut().ReturnAsync(new ReturnRequest());

            Assert.Same(returnResult, result.Body<ReturnResult>().Value);
        }

        [Fact]
        public async Task ReturnAsync_TooMany_Returns400()
        {
            _registerDataService.Setup(s => s.ProcessReturn(It.IsAny<ReturnRequest>(), _userId)).ThrowsAsync(new ArgumentException("Too many."));

            var result = await CreateSut().ReturnAsync(new ReturnRequest());

            Assert.Equal(StatusCodes.Status400BadRequest, result.Status());
        }

        [Fact]
        public async Task GetReceiptAsync_NotFound_Returns404()
        {
            _cashRegisterLogsService.Setup(s => s.GetByReceiptIdAsync("9000000001")).ThrowsAsync(new KeyNotFoundException("x"));

            var result = await CreateSut().GetReceiptAsync("9000000001");

            Assert.Equal(StatusCodes.Status404NotFound, result.Status());
        }

        [Theory]
        [InlineData("12345678")]
        [InlineData("50000000a")]
        public async Task GetProductAsync_NotABarcode_Returns400(string input)
        {
            var result = await CreateSut().GetProductAsync(input);

            Assert.Equal(StatusCodes.Status400BadRequest, result.Status());
            _productService.Verify(s => s.GetProductByBarcodeAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetProductAsync_Barcode_ReturnsProduct()
        {
            _productService.Setup(s => s.GetProductByBarcodeAsync(500000001)).ReturnsAsync(new ProductDto { ProductName = "Hamer" });

            var result = await CreateSut().GetProductAsync("500000001");

            Assert.Equal("Hamer", result.Body<ProductDto>().Value?.ProductName);
        }

        [Fact]
        public async Task GetCustomerAsync_NotACustomerId_Returns400()
        {
            var result = await CreateSut().GetCustomerAsync("70000001");

            Assert.Equal(StatusCodes.Status400BadRequest, result.Status());
        }

        [Fact]
        public async Task GetCustomerAsync_Unknown_Returns404()
        {
            _customerService.Setup(s => s.GetCustomerByCustomerIdAsync("80000099")).ThrowsAsync(new KeyNotFoundException("x"));

            var result = await CreateSut().GetCustomerAsync("80000099");

            Assert.Equal(StatusCodes.Status404NotFound, result.Status());
        }
    }

    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _customerService = new();

        private CustomerController CreateSut(Guid? userId = null) => new CustomerController(_customerService.Object).WithUser(userId);

        [Fact]
        public async Task RegisterAsync_Valid_ReturnsToken()
        {
            _customerService.Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterRequest>())).ReturnsAsync(new CustomerToken { JWT = "jwt" });

            var result = await CreateSut().RegisterAsync(new RegisterRequest());

            Assert.Equal(StatusCodes.Status200OK, result.Status());
            Assert.Equal("jwt", result.Body<CustomerToken>().Value?.JWT);
        }

        [Fact]
        public async Task RegisterAsync_Invalid_Returns400()
        {
            _customerService.Setup(s => s.RegisterCustomerAsync(It.IsAny<RegisterRequest>())).ThrowsAsync(new ArgumentException("Email in use."));

            var result = await CreateSut().RegisterAsync(new RegisterRequest());

            Assert.Equal(StatusCodes.Status400BadRequest, result.Status());
            Assert.Equal("Email in use.", result.Body<CustomerToken>().Message);
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_Returns401WithGenericMessage()
        {
            _customerService.Setup(s => s.LoginAsync(It.IsAny<CustomerLogin>())).ThrowsAsync(new AuthenticationException("The password is incorrect."));

            var result = await CreateSut().LoginAsync(new CustomerLogin());

            Assert.Equal(StatusCodes.Status401Unauthorized, result.Status());
            Assert.Equal("Email or password is incorrect.", result.Body<CustomerToken>().Message);
        }

        [Fact]
        public async Task LoginAsync_UnknownEmail_Returns401WithSameMessage()
        {
            _customerService.Setup(s => s.LoginAsync(It.IsAny<CustomerLogin>())).ThrowsAsync(new KeyNotFoundException("not found"));

            var result = await CreateSut().LoginAsync(new CustomerLogin());

            Assert.Equal(StatusCodes.Status401Unauthorized, result.Status());
            Assert.Equal("Email or password is incorrect.", result.Body<CustomerToken>().Message);
        }

        [Fact]
        public async Task GetMeAsync_NoUser_Returns401()
        {
            var result = await CreateSut().GetMeAsync();

            Assert.Equal(StatusCodes.Status401Unauthorized, result.Status());
        }

        [Fact]
        public async Task GetMeAsync_LoggedIn_ReturnsOwnAccount()
        {
            var userId = Guid.NewGuid();
            _customerService.Setup(s => s.GetCustomerByIdAsync(userId)).ReturnsAsync(new CustomerDto { Id = userId, CustomerId = "80000001", FullName = "Jan", Email = "jan@example.com" });

            var result = await CreateSut(userId).GetMeAsync();

            Assert.Equal(userId, result.Body<CustomerDto>().Value?.Id);
        }

        [Fact]
        public async Task ChangePasswordAsync_WrongCurrent_Returns400()
        {
            var userId = Guid.NewGuid();
            _customerService.Setup(s => s.ChangePasswordAsync(userId, It.IsAny<ChangePasswordRequest>())).ThrowsAsync(new ArgumentException("wrong"));

            var result = await CreateSut(userId).ChangePasswordAsync(new ChangePasswordRequest());

            Assert.Equal(StatusCodes.Status400BadRequest, result.Status());
        }
    }

    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeService> _employeeService = new();

        [Fact]
        public async Task LoginAsync_WrongPassword_Returns401WithGenericMessage()
        {
            _employeeService.Setup(s => s.LoginAsync(It.IsAny<EmployeeLogin>())).ThrowsAsync(new AuthenticationException("x"));
            var sut = new EmployeeController(_employeeService.Object).WithUser(null);

            var result = await sut.LoginAsync(new EmployeeLogin());

            Assert.Equal(StatusCodes.Status401Unauthorized, result.Status());
            Assert.Equal("Employee Id or password is incorrect.", result.Body<EmployeeToken>().Message);
        }

        [Fact]
        public async Task LoginAsync_Valid_ReturnsToken()
        {
            _employeeService.Setup(s => s.LoginAsync(It.IsAny<EmployeeLogin>())).ReturnsAsync(new EmployeeToken { JWT = "jwt" });
            var sut = new EmployeeController(_employeeService.Object).WithUser(null);

            var result = await sut.LoginAsync(new EmployeeLogin());

            Assert.Equal("jwt", result.Body<EmployeeToken>().Value?.JWT);
        }

        [Fact]
        public async Task CreateAsync_NotLoggedIn_Returns401()
        {
            var sut = new EmployeeController(_employeeService.Object).WithUser(null);

            var result = await sut.CreateAsync(new CreateEmployeeDto());

            Assert.Equal(StatusCodes.Status401Unauthorized, result.Status());
        }
    }
}
