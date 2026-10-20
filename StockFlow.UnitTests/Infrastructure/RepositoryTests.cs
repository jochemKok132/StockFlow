using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories;
using StockFlow.Infrastructure.Repositories.AuditLogs;
using StockFlow.Infrastructure.Repositories.Stock;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Infrastructure
{
    public abstract class RepositoryTestBase : IDisposable
    {
        private readonly SqliteConnection _connection;
        protected readonly ApplicationDBContext Context;

        protected RepositoryTestBase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseSqlite(_connection)
                .Options;

            Context = new ApplicationDBContext(options);
            Context.GetService<IRelationalDatabaseCreator>().CreateTables();
        }

        protected async Task<Employee> AddEmployeeAsync(string employeeId = "000001", EmployeeRole role = EmployeeRole.Cashier)
        {
            var employee = TestData.Employee(role);
            employee.EmployeeId = employeeId;
            Context.Employees.Add(employee);
            await Context.SaveChangesAsync();
            return employee;
        }

        protected async Task<Product> AddProductAsync(int barcode, params string[] tags)
        {
            var brand = TestData.Brand();
            var shelf = TestData.Shelf();
            var product = TestData.Product(brand: brand, shelf: shelf);
            product.Barcode = barcode;
            product.SalesTags = tags;
            Context.Products.Add(product);
            await Context.SaveChangesAsync();
            return product;
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public class CustomerRepositoryTests : RepositoryTestBase
    {
        private readonly CustomerRepository _sut;

        public CustomerRepositoryTests()
        {
            _sut = new CustomerRepository(Context);
        }

        [Fact]
        public async Task GetNewCustomerIdAsync_NoCustomers_ReturnsFirstCustomerId()
        {
            var result = await _sut.GetNewCustomerIdAsync();

            Assert.Equal(IdentifierFormats.FirstCustomerId.ToString(), result);
        }

        [Fact]
        public async Task GetNewCustomerIdAsync_ExistingCustomers_ReturnsHighestPlusOne()
        {
            var customer = TestData.Customer();
            customer.CustomerId = "80000041";
            await _sut.AddAsync(customer);

            var result = await _sut.GetNewCustomerIdAsync();

            Assert.Equal("80000042", result);
        }

        [Fact]
        public async Task GetCustomerByEmailAsync_DifferentCaseAndSpaces_FindsCustomer()
        {
            await _sut.AddAsync(TestData.Customer());

            var result = await _sut.GetCustomerByEmailAsync("  JAN@example.COM ");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetCustomerByEmailAsync_SoftDeleted_ReturnsNull()
        {
            await _sut.AddAsync(TestData.Customer(softDeleted: true));

            var result = await _sut.GetCustomerByEmailAsync("jan@example.com");

            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_Customer_SetsCreatedAt()
        {
            var customer = TestData.Customer();

            await _sut.AddAsync(customer);

            Assert.NotEqual(default, customer.CreatedAt);
        }
    }

    public class EmployeeRepositoryTests : RepositoryTestBase
    {
        [Fact]
        public async Task GetNewEmployeeIdAsync_TwoEmployees_ReturnsThirdSixDigitId()
        {
            await AddEmployeeAsync("000001");
            await AddEmployeeAsync("000002");
            var sut = new EmployeeRepository(Context);

            var result = await sut.GetNewEmployeeIdAsync();

            Assert.Equal("000003", result);
        }

        [Fact]
        public async Task GetEmployeeByEmployeeIdAsync_Exists_ReturnsEmployee()
        {
            await AddEmployeeAsync("000007");
            var sut = new EmployeeRepository(Context);

            var result = await sut.GetEmployeeByEmployeeIdAsync("000007");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllEmployeesAsync_WithoutTesters_HidesTester()
        {
            await AddEmployeeAsync("000001");
            await AddEmployeeAsync("000002", EmployeeRole.Tester);
            var sut = new EmployeeRepository(Context);

            var result = await sut.GetAllEmployeesAsync(new EmployeePaginationDto { PageNumber = 1, PageSize = 10 }, includeTesters: false);

            Assert.Equal("000001", Assert.Single(result).EmployeeId);
        }

        [Fact]
        public async Task GetAllEmployeesAsync_WithTesters_ReturnsTester()
        {
            await AddEmployeeAsync("000001");
            await AddEmployeeAsync("000002", EmployeeRole.Tester);
            var sut = new EmployeeRepository(Context);

            var result = await sut.GetAllEmployeesAsync(new EmployeePaginationDto { PageNumber = 1, PageSize = 10 }, includeTesters: true);

            Assert.Equal(2, result.Count());
        }
    }

    public class EmployeeLogsRepositoryTests : RepositoryTestBase
    {
        private async Task AddLogAsync(Employee by, string identifier, LogType logType)
        {
            Context.EmployeeLogs.Add(new EmployeeLogs
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                EmployeeId = by.Id,
                Identifier = identifier,
                LogType = logType,
            });
            await Context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllEmployeeLogsAsync_WithoutTesters_HidesLogsByAndAboutTesters()
        {
            var admin = await AddEmployeeAsync("000001", EmployeeRole.Admin);
            var tester = await AddEmployeeAsync("000023", EmployeeRole.Tester);
            await AddLogAsync(admin, "000001", LogType.Login);
            await AddLogAsync(tester, "000023", LogType.Login);
            await AddLogAsync(admin, "000023", LogType.Create);
            var sut = new EmployeeLogsRepository(Context);

            var result = await sut.GetAllEmployeeLogsAsync(new EmployeeLogsPaginationDto { PageNumber = 1, PageSize = 10 }, includeTesters: false);

            var log = Assert.Single(result);
            Assert.Equal("000001", log.Identifier);
        }

        [Fact]
        public async Task GetAllEmployeeLogsAsync_WithTesters_ReturnsEverything()
        {
            var admin = await AddEmployeeAsync("000001", EmployeeRole.Admin);
            var tester = await AddEmployeeAsync("000023", EmployeeRole.Tester);
            await AddLogAsync(tester, "000023", LogType.Login);
            await AddLogAsync(admin, "000023", LogType.Create);
            var sut = new EmployeeLogsRepository(Context);

            var result = await sut.GetAllEmployeeLogsAsync(new EmployeeLogsPaginationDto { PageNumber = 1, PageSize = 10 }, includeTesters: true);

            Assert.Equal(2, result.Count());
        }
    }

    public class ProductRepositoryTests : RepositoryTestBase
    {
        [Fact]
        public async Task GetNewBarcode_NoProducts_StartsAt500000001()
        {
            var sut = new ProductRepository(Context);

            var result = await sut.GetNewBarcode();

            Assert.Equal(500000001, result);
        }

        [Fact]
        public async Task GetNewBarcode_ExistingProducts_ReturnsHighestPlusOne()
        {
            await AddProductAsync(500000010);
            var sut = new ProductRepository(Context);

            var result = await sut.GetNewBarcode();

            Assert.Equal(500000011, result);
        }

        [Fact]
        public async Task GetProductByBarcodeAsync_Exists_IncludesBrandAndShelf()
        {
            await AddProductAsync(500000020);
            Context.ChangeTracker.Clear();
            var sut = new ProductRepository(Context);

            var result = await sut.GetProductByBarcodeAsync(500000020);

            Assert.NotNull(result);
            Assert.NotNull(result.Brand);
            Assert.NotNull(result.Shelf);
        }

        [Fact]
        public async Task GetAllProductsWithTagsAsync_SaleTags_ReturnsProductsHavingAllTags()
        {
            var both = await AddProductAsync(500000030, "tuin", "gereedschap");
            await AddProductAsync(500000031, "tuin");
            var sut = new ProductRepository(Context);

            var result = await sut.GetAllProductsWithTagsAsync(["tuin", "gereedschap"]);

            Assert.Equal(both.Id, Assert.Single(result).Id);
        }

        [Fact]
        public async Task SoftDeleteAsync_Product_IsHiddenFromList()
        {
            var product = await AddProductAsync(500000040);
            var sut = new ProductRepository(Context);

            await sut.SoftDeleteAsync(product);
            var result = await sut.GetAllProductsAsync(TestData.ProductPagination());

            Assert.Empty(result);
            Assert.True(product.SoftDeleted);
        }
    }

    public class SalesRepositoryTests : RepositoryTestBase
    {
        [Fact]
        public async Task GetAllSalesForProductTags_ProductTags_ReturnsSalesWhoseTagsAreAllPresent()
        {
            var matching = TestData.Sale(10, "tuin");
            var notMatching = TestData.Sale(20, "tuin", "verf");
            Context.Sales.AddRange(matching, notMatching);
            await Context.SaveChangesAsync();
            var sut = new SalesRepository(Context);

            var result = await sut.GetAllSalesForProductTags(["tuin", "gereedschap"]);

            Assert.Equal(matching.Id, Assert.Single(result).Id);
        }
    }

    public class CashRegisterLogsRepositoryTests : RepositoryTestBase
    {
        private async Task<CashRegisterLogs> AddLogAsync(CashRegisterLogsRepository sut, Employee employee, string receiptId, Product? product = null)
        {
            var log = new CashRegisterLogs { Id = Guid.NewGuid(), ReceiptId = receiptId, EmployeeId = employee.Id };
            if (product is not null)
                log.ProductsSold.Add(new RegisterItemDetail { Id = Guid.NewGuid(), CashregisterLogId = log.Id, ProductId = product.Id, Amount = 2, PriceAtTimeOfSale = 5m });
            await sut.AddAsync(log);
            return log;
        }

        [Fact]
        public async Task GetNewReceiptIdAsync_NoReceipts_ReturnsFirstReceiptId()
        {
            var sut = new CashRegisterLogsRepository(Context);

            var result = await sut.GetNewReceiptIdAsync();

            Assert.Equal("9000000001", result);
        }

        [Fact]
        public async Task GetNewReceiptIdAsync_ExistingReceipts_ReturnsHighestPlusOne()
        {
            var employee = await AddEmployeeAsync();
            var sut = new CashRegisterLogsRepository(Context);
            await AddLogAsync(sut, employee, "9000000009");
            await AddLogAsync(sut, employee, "9000000010");

            var result = await sut.GetNewReceiptIdAsync();

            Assert.Equal("9000000011", result);
        }

        [Fact]
        public async Task GetByReceiptIdAsync_Exists_IncludesLinesAndProducts()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000050);
            var sut = new CashRegisterLogsRepository(Context);
            await AddLogAsync(sut, employee, "9000000001", product);
            Context.ChangeTracker.Clear();

            var result = await sut.GetByReceiptIdAsync("9000000001");

            Assert.NotNull(result);
            var line = Assert.Single(result.ProductsSold);
            Assert.Equal(product.Id, line.Product?.Id);
        }

        [Fact]
        public async Task GetByReceiptIdAsync_Unknown_ReturnsNull()
        {
            var sut = new CashRegisterLogsRepository(Context);

            Assert.Null(await sut.GetByReceiptIdAsync("9999999999"));
        }

        [Fact]
        public async Task GetAllCashRegisterLogsAsync_ReceiptFilter_ReturnsOnlyMatching()
        {
            var employee = await AddEmployeeAsync();
            var sut = new CashRegisterLogsRepository(Context);
            await AddLogAsync(sut, employee, "9000000001");
            await AddLogAsync(sut, employee, "9000000002");

            var result = await sut.GetAllCashRegisterLogsAsync(new CashRegisterLogsPaginationDto { PageNumber = 1, PageSize = 10, ReceiptId = "0002" });

            Assert.Equal("9000000002", Assert.Single(result).ReceiptId);
        }

        [Fact]
        public async Task AddAsync_DuplicateReceiptId_Throws()
        {
            var employee = await AddEmployeeAsync();
            var sut = new CashRegisterLogsRepository(Context);
            await AddLogAsync(sut, employee, "9000000001");

            await Assert.ThrowsAsync<DbUpdateException>(() => AddLogAsync(sut, employee, "9000000001"));
        }
    }
}
