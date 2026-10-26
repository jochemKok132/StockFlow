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
        public async Task RequestMovingSaleAsync_ValidAddress_WaitsForApprovalWithoutDiscount()
        {
            var customer = TestData.Customer();
            CreateCustomerLogsDto? log = null;
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            _customerLogsService.Setup(s => s.CreateCustomerLogsAsync(It.IsAny<CreateCustomerLogsDto>())).Callback<CreateCustomerLogsDto>(l => log = l);

            var result = await _sut.RequestMovingSaleAsync(customer.Id, new MovingSaleRequest { PostalCode = "7071ab", HouseNumber = "3a" });

            Assert.Equal(MovingSaleStatus.Pending, customer.MovingSaleStatus);
            Assert.Equal("7071 AB", customer.MovingSalePostalCode);
            Assert.Equal("3A", customer.MovingSaleHouseNumber);
            Assert.Equal("1234 AB", customer.PostalCode);
            Assert.Null(customer.MovingSaleUntil);
            Assert.Equal(MovingSaleStatusDto.Pending, result.MovingSaleStatus);
            _customerRepository.Verify(r => r.UpdateAsync(customer), Times.Once);
            Assert.NotNull(log);
            Assert.Contains("MovingSale: Requested", log.Messages);
        }

        [Fact]
        public async Task RequestMovingSaleAsync_AlreadyPending_ThrowsArgumentException()
        {
            var customer = TestData.Customer();
            customer.MovingSaleStatus = MovingSaleStatus.Pending;
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.RequestMovingSaleAsync(customer.Id, new MovingSaleRequest { PostalCode = "7071 AB", HouseNumber = "3" }));
            _customerRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        private Customer PendingCustomer()
        {
            var customer = TestData.Customer();
            customer.MovingSaleStatus = MovingSaleStatus.Pending;
            customer.MovingSalePostalCode = "7071 AB";
            customer.MovingSaleHouseNumber = "3A";
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            return customer;
        }

        [Fact]
        public async Task ApproveMovingSaleAsync_Pending_MovesAddressStartsSaleAndLogs()
        {
            var customer = PendingCustomer();
            CreateCustomerLogsDto? log = null;
            _customerLogsService.Setup(s => s.CreateCustomerLogsAsync(It.IsAny<CreateCustomerLogsDto>())).Callback<CreateCustomerLogsDto>(l => log = l);

            var result = await _sut.ApproveMovingSaleAsync(customer.Id, "000003");

            Assert.Equal("7071 AB", customer.PostalCode);
            Assert.Equal("3A", customer.HouseNumber);
            Assert.Equal(MovingSaleStatus.Approved, customer.MovingSaleStatus);
            Assert.Equal(DateTime.Today.AddDays(SalePricing.MovingSaleDays), customer.MovingSaleUntil);
            Assert.Equal("000003", customer.MovingSaleReviewedBy);
            Assert.Equal(customer.MovingSaleUntil, result.MovingSaleUntil);
            Assert.NotNull(log);
            Assert.Contains("PostalCode: New: 7071 AB. Old: 1234 AB", log.Messages);
            Assert.Contains(log.Messages, m => m.StartsWith("MovingSale: Approved by 000003"));
        }

        [Fact]
        public async Task RejectMovingSaleAsync_Pending_KeepsAddressAndGivesNoSale()
        {
            var customer = PendingCustomer();

            await _sut.RejectMovingSaleAsync(customer.Id, "000003");

            Assert.Equal(MovingSaleStatus.Rejected, customer.MovingSaleStatus);
            Assert.Equal("1234 AB", customer.PostalCode);
            Assert.Null(customer.MovingSaleUntil);
            _customerRepository.Verify(r => r.UpdateAsync(customer), Times.Once);
        }

        [Theory]
        [InlineData(MovingSaleStatus.None)]
        [InlineData(MovingSaleStatus.Approved)]
        [InlineData(MovingSaleStatus.Rejected)]
        public async Task ApproveMovingSaleAsync_NotPending_ThrowsArgumentException(MovingSaleStatus status)
        {
            var customer = TestData.Customer();
            customer.MovingSaleStatus = status;
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ApproveMovingSaleAsync(customer.Id, "000003"));
            _customerRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task ApproveMovingSaleAsync_UnknownCustomer_ThrowsKeyNotFoundException()
        {
            _customerRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Customer?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.ApproveMovingSaleAsync(Guid.NewGuid(), "000003"));
        }

        [Fact]
        public async Task GetMovingSaleRequestsAsync_ReturnsPendingCustomers()
        {
            _customerRepository.Setup(r => r.GetPendingMovingSalesAsync()).ReturnsAsync(new[] { PendingCustomer() });

            var result = await _sut.GetMovingSaleRequestsAsync();

            Assert.Equal("7071 AB", Assert.Single(result).MovingSalePostalCode);
        }

        [Fact]
        public async Task RequestMovingSaleAsync_AlreadyActive_ThrowsArgumentException()
        {
            var customer = TestData.Customer();
            customer.MovingSaleUntil = DateTime.Today.AddDays(10);
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.RequestMovingSaleAsync(customer.Id, new MovingSaleRequest { PostalCode = "7071 AB", HouseNumber = "3" }));
            _customerRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Theory]
        [InlineData("123 AB", "3")]
        [InlineData("1234 AB", "")]
        [InlineData("1234 AB", "abc")]
        public async Task RequestMovingSaleAsync_InvalidAddress_ThrowsArgumentException(string postalCode, string houseNumber)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.RequestMovingSaleAsync(Guid.NewGuid(), new MovingSaleRequest { PostalCode = postalCode, HouseNumber = houseNumber }));
        }

        [Fact]
        public async Task RequestMovingSaleAsync_UnknownCustomer_ThrowsKeyNotFoundException()
        {
            _customerRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Customer?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.RequestMovingSaleAsync(Guid.NewGuid(), new MovingSaleRequest { PostalCode = "7071 AB", HouseNumber = "3" }));
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
            var request = new CustomerLogin { Email = "no-email", Password = "x" };

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

            await Assert.ThrowsAsync<AuthenticationException>(() => _sut.LoginAsync(new CustomerLogin { Email = customer.Email, Password = "Welcome123!" }));
            _password.Verify(p => p.Validate(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_CorrectPassword_ReturnsTokenAndWritesLoginLog()
        {
            var customer = TestData.Customer();
            var token = new CustomerToken { JWT = "jwt" };
            CreateCustomerLogsDto? log = null;
            _customerRepository.Setup(r => r.GetCustomerByEmailAsync(customer.Email)).ReturnsAsync(customer);
            _password.Setup(p => p.Validate(customer.HashedPassword, "Welcome123!")).Returns(true);
            _authenticationService.Setup(a => a.GenerateToken(customer)).Returns(token);
            _customerLogsService.Setup(s => s.CreateCustomerLogsAsync(It.IsAny<CreateCustomerLogsDto>())).Callback<CreateCustomerLogsDto>(l => log = l);

            var result = await _sut.LoginAsync(new CustomerLogin { Email = customer.Email, Password = "Welcome123!" });

            Assert.Same(token, result);
            Assert.NotNull(log);
            Assert.Equal(LogTypeDto.Login, log.LogType);
            Assert.Equal(customer.Id, log.CustomerId);
        }

        [Fact]
        public async Task RegisterCustomerAsync_PasswordsDoNotMatch_ThrowsArgumentException()
        {
            var request = TestData.RegisterRequest();
            request.ConfirmPassword = "Other123!";

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
            _password.Setup(p => p.Hash("Welcome123!")).Returns("hashed");
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
            var request = new ChangePasswordRequest { CurrentPassword = "Old12345!", NewPassword = "NewPass1!", ConfirmPassword = "NewPass1!" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.ChangePasswordAsync(Guid.NewGuid(), request));
        }

        [Fact]
        public async Task ChangePasswordAsync_WrongCurrentPassword_ThrowsArgumentException()
        {
            var customer = TestData.Customer();
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            _password.Setup(p => p.Validate(customer.HashedPassword, "Old12345!")).Returns(false);
            var request = new ChangePasswordRequest { CurrentPassword = "Old12345!", NewPassword = "NewPass1!", ConfirmPassword = "NewPass1!" };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.ChangePasswordAsync(customer.Id, request));
            _customerRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task ChangePasswordAsync_CorrectCurrentPassword_StoresNewHashAndLogs()
        {
            var customer = TestData.Customer();
            _customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            _password.Setup(p => p.Validate("hash", "Old12345!")).Returns(true);
            _password.Setup(p => p.Hash("NewPass1!")).Returns("new-hash");
            var request = new ChangePasswordRequest { CurrentPassword = "Old12345!", NewPassword = "NewPass1!", ConfirmPassword = "NewPass1!" };

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
