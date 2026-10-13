using StockFlow.Application.Services.ManagementApp;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepository = new();
        private readonly Mock<IAuthenticationService> _authenticationService = new();
        private readonly Mock<IPassword> _password = new();
        private readonly Mock<ICustomerLogsService> _customerLogsService = new();
        private readonly CustomerService _sut;

        public CustomerServiceTests()
        {
            _sut = new CustomerService(_customerRepository.Object, _authenticationService.Object, _password.Object, _customerLogsService.Object);
        }

        [Fact]
        public async Task GetAllCustomersAsync_ValidPagination_ReturnsMappedCustomers()
        {
            var customers = new[] { TestData.Customer(), TestData.Customer() };
            _customerRepository.Setup(r => r.GetAllCustomersAsync(It.IsAny<CustomerPaginationDto>())).ReturnsAsync(customers);

            var result = await _sut.GetAllCustomersAsync(new CustomerPaginationDto { PageNumber = 1, PageSize = 10 });

            Assert.Equal(2, result.Count);
            Assert.Equal(customers[0].Id, result[0].Id);
        }

        [Fact]
        public async Task GetAllCustomersAsync_PageSizeTooBig_ThrowsArgumentException()
        {
            var pagination = new CustomerPaginationDto { PageNumber = 1, PageSize = 1000 };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetAllCustomersAsync(pagination));
        }

        [Fact]
        public async Task GetCustomerByEmailAsync_CustomerNotFound_ThrowsKeyNotFoundException()
        {
            _customerRepository.Setup(r => r.GetCustomerByEmailAsync("x@example.com")).ReturnsAsync((Customer?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetCustomerByEmailAsync("x@example.com"));
        }

        [Fact]
        public async Task GetCustomerByEmailAsync_CustomerFound_ReturnsDto()
        {
            var customer = TestData.Customer();
            _customerRepository.Setup(r => r.GetCustomerByEmailAsync(customer.Email)).ReturnsAsync(customer);

            var result = await _sut.GetCustomerByEmailAsync(customer.Email);

            Assert.Equal(customer.CustomerId, result.CustomerId);
        }

        [Fact]
        public async Task GetCustomerByCustomerIdAsync_CustomerNotFound_ThrowsKeyNotFoundException()
        {
            _customerRepository.Setup(r => r.GetCustomerByCustomerIdAsync("80000099")).ReturnsAsync((Customer?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetCustomerByCustomerIdAsync("80000099"));
        }

        [Fact]
        public async Task GetCustomerByCustomerIdAsync_CustomerFound_ReturnsDto()
        {
            var customer = TestData.Customer();
            _customerRepository.Setup(r => r.GetCustomerByCustomerIdAsync(customer.CustomerId)).ReturnsAsync(customer);

            var result = await _sut.GetCustomerByCustomerIdAsync(customer.CustomerId);

            Assert.Equal(customer.FullName, result.FullName);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_SoftDeleted_ThrowsKeyNotFoundException()
        {
            var customer = TestData.Customer(softDeleted: true);
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetCustomerByIdAsync(customer.Id));
        }

        [Fact]
        public async Task GetCustomerByIdAsync_CustomerFound_ReturnsDto()
        {
            var customer = TestData.Customer(savedPoints: 250);
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

            var result = await _sut.GetCustomerByIdAsync(customer.Id);

            Assert.Equal(250, result.SavedPoints);
        }

        [Fact]
        public async Task LoginAsync_InvalidEmail_ThrowsArgumentException()
        {
            var request = new CustomerLogin { Email = "geen-email", Password = "x" };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_CustomerNotFound_ThrowsKeyNotFoundException()
        {
            _customerRepository.Setup(r => r.GetCustomerByEmailAsync(It.IsAny<string>())).ReturnsAsync((Customer?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.LoginAsync(new CustomerLogin { Email = "jan@example.com", Password = "x" }));
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ThrowsAuthenticationException()
        {
            var customer = TestData.Customer();
            _customerRepository.Setup(r => r.GetCustomerByEmailAsync(customer.Email)).ReturnsAsync(customer);
            _password.Setup(p => p.Validate(customer.HashedPassword, "wrong")).Returns(false);

            await Assert.ThrowsAsync<AuthenticationException>(() => _sut.LoginAsync(new CustomerLogin { Email = customer.Email, Password = "wrong" }));
        }

        [Fact]
        public async Task LoginAsync_NoPasswordHash_ThrowsAuthenticationException()
        {
            var customer = TestData.Customer(hashedPassword: "");
            _customerRepository.Setup(r => r.GetCustomerByEmailAsync(customer.Email)).ReturnsAsync(customer);

            await Assert.ThrowsAsync<AuthenticationException>(() => _sut.LoginAsync(new CustomerLogin { Email = customer.Email, Password = "Welkom123!" }));
            _password.Verify(p => p.Validate(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_CorrectPassword_ReturnsTokenAndWritesLoginLog()
        {
            var customer = TestData.Customer();
            var token = new CustomerToken { JWT = "jwt" };
            CreateCustomerLogsDto? log = null;
            _customerRepository.Setup(r => r.GetCustomerByEmailAsync(customer.Email)).ReturnsAsync(customer);
            _password.Setup(p => p.Validate(customer.HashedPassword, "Welkom123!")).Returns(true);
            _authenticationService.Setup(a => a.GenerateToken(customer)).Returns(token);
            _customerLogsService.Setup(s => s.CreateCustomerLogsAsync(It.IsAny<CreateCustomerLogsDto>())).Callback<CreateCustomerLogsDto>(l => log = l);

            var result = await _sut.LoginAsync(new CustomerLogin { Email = customer.Email, Password = "Welkom123!" });

            Assert.Same(token, result);
            Assert.NotNull(log);
            Assert.Equal(LogTypeDto.Login, log.LogType);
            Assert.Equal(customer.Id, log.CustomerId);
        }

        [Fact]
        public async Task RegisterCustomerAsync_PasswordsDoNotMatch_ThrowsArgumentException()
        {
            var request = TestData.RegisterRequest();
            request.ConfirmPassword = "Anders123!";

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.RegisterCustomerAsync(request));
            _customerRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task RegisterCustomerAsync_EmailAlreadyUsed_ThrowsArgumentException()
        {
            _customerRepository.Setup(r => r.GetCustomerByEmailAsync("jan@example.com")).ReturnsAsync(TestData.Customer());

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.RegisterCustomerAsync(TestData.RegisterRequest()));
            _customerRepository.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task RegisterCustomerAsync_ValidRequest_CreatesNormalizedCustomerWithWelcomePoints()
        {
            Customer? created = null;
            CreateCustomerLogsDto? log = null;
            var token = new CustomerToken { JWT = "jwt" };
            _customerRepository.Setup(r => r.GetCustomerByEmailAsync(It.IsAny<string>())).ReturnsAsync((Customer?)null);
            _customerRepository.Setup(r => r.GetNewCustomerIdAsync()).ReturnsAsync("80000016");
            _customerRepository.Setup(r => r.AddAsync(It.IsAny<Customer>())).Callback<Customer>(c => created = c);
            _password.Setup(p => p.Hash("Welkom123!")).Returns("hashed");
            _authenticationService.Setup(a => a.GenerateToken(It.IsAny<Customer>())).Returns(token);
            _customerLogsService.Setup(s => s.CreateCustomerLogsAsync(It.IsAny<CreateCustomerLogsDto>())).Callback<CreateCustomerLogsDto>(l => log = l);

            var result = await _sut.RegisterCustomerAsync(TestData.RegisterRequest());

            Assert.Same(token, result);
            Assert.NotNull(created);
            Assert.Equal("80000016", created.CustomerId);
            Assert.Equal("Jan Jansen", created.FullName);
            Assert.Equal("jan@example.com", created.Email);
            Assert.Equal("1234 AB", created.PostalCode);
            Assert.Equal("12A", created.HouseNumber);
            Assert.Equal("hashed", created.HashedPassword);
            Assert.Equal(LoyaltyPoints.WelcomePoints, created.SavedPoints);
            Assert.NotNull(log);
            Assert.Equal(LogTypeDto.Create, log.LogType);
        }

        [Fact]
        public async Task ChangePasswordAsync_CustomerNotFound_ThrowsKeyNotFoundException()
        {
            _customerRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Customer?)null);
            var request = new ChangePasswordRequest { CurrentPassword = "Oud12345!", NewPassword = "Nieuw123!", ConfirmPassword = "Nieuw123!" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.ChangePasswordAsync(Guid.NewGuid(), request));
        }

        [Fact]
        public async Task ChangePasswordAsync_WrongCurrentPassword_ThrowsArgumentException()
        {
            var customer = TestData.Customer();
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            _password.Setup(p => p.Validate(customer.HashedPassword, "Oud12345!")).Returns(false);
            var request = new ChangePasswordRequest { CurrentPassword = "Oud12345!", NewPassword = "Nieuw123!", ConfirmPassword = "Nieuw123!" };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ChangePasswordAsync(customer.Id, request));
            _customerRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task ChangePasswordAsync_CorrectCurrentPassword_StoresNewHashAndLogs()
        {
            var customer = TestData.Customer();
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            _password.Setup(p => p.Validate("hash", "Oud12345!")).Returns(true);
            _password.Setup(p => p.Hash("Nieuw123!")).Returns("new-hash");
            var request = new ChangePasswordRequest { CurrentPassword = "Oud12345!", NewPassword = "Nieuw123!", ConfirmPassword = "Nieuw123!" };

            await _sut.ChangePasswordAsync(customer.Id, request);

            Assert.Equal("new-hash", customer.HashedPassword);
            _customerRepository.Verify(r => r.UpdateAsync(customer), Times.Once);
            _customerLogsService.Verify(s => s.CreateCustomerLogsAsync(It.Is<CreateCustomerLogsDto>(l => l.Messages.Contains("Password: Changed."))), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerAsync_CustomerNotFound_ThrowsKeyNotFoundException()
        {
            var dto = new UpdateCustomerDto { Id = Guid.NewGuid(), FullName = "Jan", Email = "jan@example.com" };
            _customerRepository.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Customer?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateCustomerAsync(dto));
        }

        [Fact]
        public async Task UpdateCustomerAsync_ValidUpdate_UpdatesCustomerAndLogsChanges()
        {
            var customer = TestData.Customer(savedPoints: 10);
            var dto = new UpdateCustomerDto { Id = customer.Id, FullName = "Jan de Vries", Email = customer.Email, PostalCode = customer.PostalCode, HouseNumber = customer.HouseNumber, SavedPoints = 10 };
            CreateCustomerLogsDto? log = null;
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            _customerLogsService.Setup(s => s.CreateCustomerLogsAsync(It.IsAny<CreateCustomerLogsDto>())).Callback<CreateCustomerLogsDto>(l => log = l);

            await _sut.UpdateCustomerAsync(dto);

            Assert.Equal("Jan de Vries", customer.FullName);
            _customerRepository.Verify(r => r.UpdateAsync(customer), Times.Once);
            Assert.NotNull(log);
            Assert.Contains(log.Messages, m => m.StartsWith("FullName: New: Jan de Vries"));
        }
    }
}
