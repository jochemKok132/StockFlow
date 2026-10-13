using StockFlow.Application.Services.ManagementApp;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepository = new();
        private readonly Mock<IAuthenticationService> _authenticationService = new();
        private readonly Mock<IPassword> _password = new();
        private readonly Mock<IEmployeeLogsService> _employeeLogsService = new();
        private readonly EmployeeService _sut;

        public EmployeeServiceTests()
        {
            _sut = new EmployeeService(_employeeRepository.Object, _authenticationService.Object, _password.Object, _employeeLogsService.Object);
        }

        [Fact]
        public async Task CreateEmployeeAsync_RoleNone_ThrowsArgumentException()
        {
            var dto = new CreateEmployeeDto { FullName = "Piet", Password = "1234", Role = EmployeeRoleDto.None };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateEmployeeAsync(dto, Guid.NewGuid()));
            _employeeRepository.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
        }

        [Fact]
        public async Task CreateEmployeeAsync_ValidEmployee_HashesPasswordAssignsIdAndLogs()
        {
            Employee? created = null;
            var userId = Guid.NewGuid();
            _employeeRepository.Setup(r => r.GetNewEmployeeIdAsync()).ReturnsAsync("000023");
            _employeeRepository.Setup(r => r.AddAsync(It.IsAny<Employee>())).Callback<Employee>(e => created = e);
            _password.Setup(p => p.Hash("123456")).Returns("hashed");
            var dto = new CreateEmployeeDto { FullName = "Piet Pieters", Password = "123456", Role = EmployeeRoleDto.Cashier };

            await _sut.CreateEmployeeAsync(dto, userId);

            Assert.NotNull(created);
            Assert.Equal("000023", created.EmployeeId);
            Assert.Equal("hashed", created.HashedPassword);
            Assert.Equal(EmployeeRole.Cashier, created.Role);
            _employeeLogsService.Verify(s => s.CreateEmployeeLogsAsync(It.Is<CreateEmployeeLogsDto>(l => l.LogType == LogTypeDto.Create && l.EmployeeId == userId)), Times.Once);
        }

        [Fact]
        public async Task GetAllEmployeesAsync_ValidPagination_ReturnsMappedEmployees()
        {
            _employeeRepository.Setup(r => r.GetAllEmployeesAsync(It.IsAny<EmployeePaginationDto>())).ReturnsAsync(new[] { TestData.Employee(EmployeeRole.Manager) });

            var result = await _sut.GetAllEmployeesAsync(new EmployeePaginationDto { PageNumber = 1, PageSize = 10 });

            Assert.Single(result);
            Assert.Equal(EmployeeRoleDto.Manager, result[0].Role);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_EmployeeNotFound_ThrowsKeyNotFoundException()
        {
            _employeeRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Employee?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetEmployeeByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_EmployeeFound_ReturnsDto()
        {
            var employee = TestData.Employee();
            _employeeRepository.Setup(r => r.GetByIdAsync(employee.Id)).ReturnsAsync(employee);

            var result = await _sut.GetEmployeeByIdAsync(employee.Id);

            Assert.Equal(employee.EmployeeId, result.EmployeeId);
        }

        [Fact]
        public async Task GetEmployeeByEmployeeIdAsync_EmployeeNotFound_ThrowsKeyNotFoundException()
        {
            _employeeRepository.Setup(r => r.GetEmployeeByEmployeeIdAsync("999999")).ReturnsAsync((Employee?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetEmployeeByEmployeeIdAsync("999999"));
        }

        [Fact]
        public async Task GetEmployeeByEmployeeIdAsync_EmployeeFound_ReturnsDto()
        {
            var employee = TestData.Employee();
            _employeeRepository.Setup(r => r.GetEmployeeByEmployeeIdAsync(employee.EmployeeId)).ReturnsAsync(employee);

            var result = await _sut.GetEmployeeByEmployeeIdAsync(employee.EmployeeId);

            Assert.Equal(employee.FullName, result.FullName);
        }

        [Fact]
        public async Task LoginAsync_EmployeeIdWithLetters_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.LoginAsync(new EmployeeLogin { EmployeeId = "00a001", Password = "001273" }));
        }

        [Fact]
        public async Task LoginAsync_EmployeeNotFound_ThrowsKeyNotFoundException()
        {
            _employeeRepository.Setup(r => r.GetEmployeeByEmployeeIdAsync("000099")).ReturnsAsync((Employee?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.LoginAsync(new EmployeeLogin { EmployeeId = "000099", Password = "001273" }));
        }

        [Fact]
        public async Task LoginAsync_NoPasswordHash_ThrowsAuthenticationException()
        {
            var employee = TestData.Employee(hashedPassword: null);
            _employeeRepository.Setup(r => r.GetEmployeeByEmployeeIdAsync(employee.EmployeeId)).ReturnsAsync(employee);

            await Assert.ThrowsAsync<AuthenticationException>(() => _sut.LoginAsync(new EmployeeLogin { EmployeeId = employee.EmployeeId, Password = "001273" }));
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ThrowsAuthenticationException()
        {
            var employee = TestData.Employee();
            _employeeRepository.Setup(r => r.GetEmployeeByEmployeeIdAsync(employee.EmployeeId)).ReturnsAsync(employee);
            _password.Setup(p => p.Validate("hash", "999999")).Returns(false);

            await Assert.ThrowsAsync<AuthenticationException>(() => _sut.LoginAsync(new EmployeeLogin { EmployeeId = employee.EmployeeId, Password = "999999" }));
            _employeeLogsService.Verify(s => s.CreateEmployeeLogsAsync(It.IsAny<CreateEmployeeLogsDto>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_CorrectPassword_ReturnsTokenAndLogsLogin()
        {
            var employee = TestData.Employee();
            var token = new EmployeeToken { JWT = "jwt" };
            _employeeRepository.Setup(r => r.GetEmployeeByEmployeeIdAsync(employee.EmployeeId)).ReturnsAsync(employee);
            _password.Setup(p => p.Validate("hash", "001273")).Returns(true);
            _authenticationService.Setup(a => a.GenerateToken(employee)).Returns(token);

            var result = await _sut.LoginAsync(new EmployeeLogin { EmployeeId = employee.EmployeeId, Password = "001273" });

            Assert.Same(token, result);
            _employeeLogsService.Verify(s => s.CreateEmployeeLogsAsync(It.Is<CreateEmployeeLogsDto>(l => l.LogType == LogTypeDto.Login)), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_EmployeeNotFound_ThrowsKeyNotFoundException()
        {
            var dto = new UpdateEmployeeDto { Id = Guid.NewGuid(), FullName = "Piet", Role = EmployeeRoleDto.Cashier };
            _employeeRepository.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Employee?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateEmployeeAsync(dto, Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateEmployeeAsync_WithNewPassword_HashesPasswordAndLogsPasswordChange()
        {
            var employee = TestData.Employee();
            CreateEmployeeLogsDto? log = null;
            _employeeRepository.Setup(r => r.GetByIdAsync(employee.Id)).ReturnsAsync(employee);
            _password.Setup(p => p.Hash("4321")).Returns("new-hash");
            _employeeLogsService.Setup(s => s.CreateEmployeeLogsAsync(It.IsAny<CreateEmployeeLogsDto>())).Callback<CreateEmployeeLogsDto>(l => log = l);
            var dto = new UpdateEmployeeDto { Id = employee.Id, FullName = employee.FullName, Role = EmployeeRoleDto.ShiftLeader, NewPassword = "4321" };

            await _sut.UpdateEmployeeAsync(dto, Guid.NewGuid());

            Assert.Equal("new-hash", employee.HashedPassword);
            Assert.Equal(EmployeeRole.ShiftLeader, employee.Role);
            _employeeRepository.Verify(r => r.UpdateAsync(employee), Times.Once);
            Assert.NotNull(log);
            Assert.Contains("Password: Changed.", log.Messages);
            Assert.DoesNotContain(log.Messages, m => m.Contains("4321") || m.Contains("new-hash"));
        }

        [Fact]
        public async Task UpdateEmployeeAsync_WithoutNewPassword_KeepsPassword()
        {
            var employee = TestData.Employee();
            _employeeRepository.Setup(r => r.GetByIdAsync(employee.Id)).ReturnsAsync(employee);
            var dto = new UpdateEmployeeDto { Id = employee.Id, FullName = "Nieuwe Naam", Role = EmployeeRoleDto.Cashier };

            await _sut.UpdateEmployeeAsync(dto, Guid.NewGuid());

            Assert.Equal("hash", employee.HashedPassword);
            Assert.Equal("Nieuwe Naam", employee.FullName);
            _password.Verify(p => p.Hash(It.IsAny<string>()), Times.Never);
        }
    }
}
