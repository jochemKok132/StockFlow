using StockFlow.Domain.Entities.Ordering;
using StockFlow.Infrastructure.Repositories;
using StockFlow.Infrastructure.Repositories.Ordering;
using StockFlow.Infrastructure.Repositories.Stock;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Infrastructure
{
    public abstract class OrderingRepositoryTestBase : RepositoryTestBase
    {
        protected async Task<Product> AddProductAsync(int barcode, Action<Product> configure)
        {
            var product = TestData.Product();
            product.Barcode = barcode;
            configure(product);
            Context.Products.Add(product);
            await Context.SaveChangesAsync();
            return product;
        }

        protected async Task<Customer> AddCustomerAsync()
        {
            var customer = TestData.Customer();
            Context.Customers.Add(customer);
            await Context.SaveChangesAsync();
            return customer;
        }

        protected async Task<CustomerOrder> AddCustomerOrderAsync(Product product, CustomerOrderStatus status, int amount, int reserved, int number = 7000001)
        {
            var customer = await AddCustomerAsync();
            var order = new CustomerOrder
            {
                Id = Guid.NewGuid(),
                OrderNumber = number,
                CustomerId = customer.Id,
                Status = status,
                Lines = [new CustomerOrderLine { ProductId = product.Id, Amount = amount, ReservedAmount = reserved }],
            };
            Context.CustomerOrders.Add(order);
            await Context.SaveChangesAsync();
            return order;
        }

        protected async Task<PurchaseOrder> AddPurchaseOrderAsync(Product product, PurchaseOrderStatus status, int amount, int number = 1001, DateTime? orderDate = null)
        {
            var order = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                OrderNumber = number,
                SupplierId = product.SupplierId,
                Status = status,
                OrderDate = orderDate ?? DateTime.Today.AddDays(3),
                Lines = [new PurchaseOrderLine { ProductId = product.Id, Amount = amount }],
            };
            Context.PurchaseOrders.Add(order);
            await Context.SaveChangesAsync();
            return order;
        }
    }

    public class ProductFilterRepositoryTests : OrderingRepositoryTestBase
    {
        [Fact]
        public async Task GetAllProductsAsync_StatusFilter_ReturnsOnlyThatStatus()
        {
            await AddProductAsync(500000101, p => p.Status = ProductStatus.Active);
            var draft = await AddProductAsync(500000102, p => p.Status = ProductStatus.Draft);
            var sut = new ProductRepository(Context);

            var pagination = TestData.ProductPagination();
            pagination.Status = ProductStatusDto.Draft;
            var result = await sut.GetAllProductsAsync(pagination);

            Assert.Equal(draft.Id, Assert.Single(result).Id);
        }

        [Fact]
        public async Task GetAllProductsAsync_BrandFilter_ReturnsOnlyThatBrand()
        {
            var first = await AddProductAsync(500000103, _ => { });
            await AddProductAsync(500000104, _ => { });
            var sut = new ProductRepository(Context);

            var pagination = TestData.ProductPagination();
            pagination.BrandId = first.BrandId;
            var result = await sut.GetAllProductsAsync(pagination);

            Assert.Equal(first.Id, Assert.Single(result).Id);
        }

        [Fact]
        public async Task GetAllProductsAsync_OnSaleFilter_SplitsProductsWithAnActiveSale()
        {
            var activeSale = TestData.Sale(10, "tools");
            var endedSale = TestData.Sale(10, "tools");
            endedSale.EndDate = DateTime.Today.AddDays(-1);
            var onSale = await AddProductAsync(500000105, p => p.Sales.Add(activeSale));
            var ended = await AddProductAsync(500000106, p => p.Sales.Add(endedSale));
            var sut = new ProductRepository(Context);

            var yes = TestData.ProductPagination();
            yes.OnSale = true;
            var no = TestData.ProductPagination();
            no.OnSale = false;

            Assert.Equal(onSale.Id, Assert.Single(await sut.GetAllProductsAsync(yes)).Id);
            Assert.Equal(ended.Id, Assert.Single(await sut.GetAllProductsAsync(no)).Id);
        }

        [Fact]
        public async Task GetAllProductsAsync_SaleFilter_ReturnsProductsOfThatSale()
        {
            var sale = TestData.Sale(10, "tools");
            var inSale = await AddProductAsync(500000107, p => p.Sales.Add(sale));
            await AddProductAsync(500000108, _ => { });
            var sut = new ProductRepository(Context);

            var pagination = TestData.ProductPagination();
            pagination.SaleId = sale.Id;
            var result = await sut.GetAllProductsAsync(pagination);

            Assert.Equal(inSale.Id, Assert.Single(result).Id);
        }

        [Fact]
        public async Task GetAllProductsAsync_BelowMinimum_CountsReservedStock()
        {
            var reserved = await AddProductAsync(500000109, p => { p.Stock = 12; p.MinimumStock = 10; });
            await AddProductAsync(500000110, p => { p.Stock = 12; p.MinimumStock = 10; });
            await AddProductAsync(500000111, p => { p.Stock = 0; p.MinimumStock = 0; });
            await AddCustomerOrderAsync(reserved, CustomerOrderStatus.ReadyForPickup, 5, 5);
            var sut = new ProductRepository(Context);

            var pagination = TestData.ProductPagination();
            pagination.BelowMinimum = true;
            var result = await sut.GetAllProductsAsync(pagination);

            Assert.Equal(reserved.Id, Assert.Single(result).Id);
        }

        [Fact]
        public async Task GetAllProductsAsync_OrderByPriceDescending_SortsByPrice()
        {
            await AddProductAsync(500000112, p => p.Price = 2.50m);
            await AddProductAsync(500000113, p => p.Price = 19.99m);
            await AddProductAsync(500000114, p => p.Price = 7.25m);
            var sut = new ProductRepository(Context);

            var pagination = TestData.ProductPagination();
            pagination.OrderBy = ProductOrderBy.Price;
            pagination.OrderType = OrderType.Descending;
            var result = await sut.GetAllProductsAsync(pagination);

            Assert.Equal([19.99m, 7.25m, 2.50m], result.Select(p => p.Price));
        }

        [Fact]
        public async Task GetReservedStockAsync_OnlyWaitingAndReadyOrdersCount()
        {
            var product = await AddProductAsync(500000115, _ => { });
            await AddCustomerOrderAsync(product, CustomerOrderStatus.Waiting, 4, 2, 7000001);
            await AddCustomerOrderAsync(product, CustomerOrderStatus.ReadyForPickup, 3, 3, 7000002);
            await AddCustomerOrderAsync(product, CustomerOrderStatus.PickedUp, 9, 9, 7000003);
            var sut = new ProductRepository(Context);

            var result = await sut.GetReservedStockAsync([product.Id]);

            Assert.Equal(5, result[product.Id]);
        }

        [Fact]
        public async Task GetOnOrderAsync_OnlyOpenAndSentOrdersCount()
        {
            var product = await AddProductAsync(500000116, _ => { });
            await AddPurchaseOrderAsync(product, PurchaseOrderStatus.Open, 4, 1001);
            await AddPurchaseOrderAsync(product, PurchaseOrderStatus.Sent, 6, 1002);
            await AddPurchaseOrderAsync(product, PurchaseOrderStatus.Received, 20, 1003);
            var sut = new ProductRepository(Context);

            var result = await sut.GetOnOrderAsync([product.Id]);

            Assert.Equal(10, result[product.Id]);
        }

        [Fact]
        public async Task GetBatchesToCountAsync_SkipsForcedEmptyAndLaterBatches()
        {
            var product = await AddProductAsync(500000117, p =>
            {
                p.CanExpire = true;
                p.Batches.Add(new StockBatch { Amount = 3, ExpiryDate = DateTime.Today.AddDays(5) });
                p.Batches.Add(new StockBatch { Amount = 3, ExpiryDate = DateTime.Today.AddDays(30) });
                p.Batches.Add(new StockBatch { Amount = 3, ExpiryDate = DateTime.Today.AddDays(2), CountForced = true });
                p.Batches.Add(new StockBatch { Amount = 0, ExpiryDate = DateTime.Today.AddDays(1) });
            });
            var sut = new ProductRepository(Context);

            var result = await sut.GetBatchesToCountAsync(DateTime.Today.AddDays(7));

            var batch = Assert.Single(result);
            Assert.Equal(DateTime.Today.AddDays(5), batch.ExpiryDate);
            Assert.Equal(product.Id, batch.ProductId);
        }

        [Fact]
        public async Task GetMinimumStockProductsAsync_AutoOrderOnly_SkipsManualAndInactive()
        {
            var auto = await AddProductAsync(500000118, p => { p.MinimumStock = 5; p.AutoOrder = true; });
            await AddProductAsync(500000119, p => { p.MinimumStock = 5; });
            await AddProductAsync(500000120, p => { p.MinimumStock = 5; p.AutoOrder = true; p.Status = ProductStatus.Disabled; });
            var sut = new ProductRepository(Context);

            var autoOnly = await sut.GetMinimumStockProductsAsync(autoOrderOnly: true);
            var all = await sut.GetMinimumStockProductsAsync(autoOrderOnly: false);

            Assert.Equal(auto.Id, Assert.Single(autoOnly).Id);
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public async Task GetByIdAsync_IncludesSupplierPricesAndBatches()
        {
            var product = await AddProductAsync(500000121, p =>
            {
                p.Prices.Add(new ProductPrice { Price = 10m, ValidFrom = DateTime.Today });
                p.Batches.Add(new StockBatch { Amount = 1, ExpiryDate = DateTime.Today.AddDays(9) });
            });
            Context.ChangeTracker.Clear();
            var sut = new ProductRepository(Context);

            var result = await sut.GetByIdAsync(product.Id);

            Assert.NotNull(result?.Supplier);
            Assert.Single(result!.Prices);
            Assert.Single(result.Batches);
        }
    }

    public class SupplierRepositoryTests : OrderingRepositoryTestBase
    {
        [Fact]
        public async Task GetAllSuppliersAsync_NameFilter_SkipsDeletedSuppliers()
        {
            var paint = TestData.Supplier();
            paint.SupplierName = "Paint Co";
            var deleted = TestData.Supplier(softDeleted: true);
            deleted.SupplierName = "Paint Old";
            Context.Suppliers.AddRange(paint, deleted, TestData.Supplier());
            await Context.SaveChangesAsync();
            var sut = new SupplierRepository(Context);

            var result = await sut.GetAllSuppliersAsync(new SupplierPaginationDto { PageNumber = 1, PageSize = 10, SupplierName = "paint" });

            Assert.Equal(paint.Id, Assert.Single(result).Id);
        }

        [Fact]
        public async Task GetProductCountsAsync_CountsActiveProductsPerSupplier()
        {
            var product = await AddProductAsync(500000130, _ => { });
            await AddProductAsync(500000131, p => { p.SupplierId = product.SupplierId; p.Supplier = null; p.SoftDeleted = true; });
            var sut = new SupplierRepository(Context);

            var result = await sut.GetProductCountsAsync([product.SupplierId]);

            Assert.Equal(1, result[product.SupplierId]);
        }
    }

    public class PurchaseOrderRepositoryTests : OrderingRepositoryTestBase
    {
        [Fact]
        public async Task GetNextOrderNumberAsync_NoOrders_StartsAt1001()
        {
            var sut = new PurchaseOrderRepository(Context);

            Assert.Equal(1001, await sut.GetNextOrderNumberAsync());
        }

        [Fact]
        public async Task GetOpenForSupplierAsync_ReturnsOnlyTheOpenOrder()
        {
            var product = await AddProductAsync(500000140, _ => { });
            await AddPurchaseOrderAsync(product, PurchaseOrderStatus.Sent, 1, 1001);
            var open = await AddPurchaseOrderAsync(product, PurchaseOrderStatus.Open, 1, 1002);
            var sut = new PurchaseOrderRepository(Context);

            var result = await sut.GetOpenForSupplierAsync(product.SupplierId);

            Assert.Equal(open.Id, result?.Id);
        }

        [Fact]
        public async Task GetOpenOrdersDueAsync_ReturnsOpenOrdersOnOrBeforeToday()
        {
            var product = await AddProductAsync(500000141, _ => { });
            var due = await AddPurchaseOrderAsync(product, PurchaseOrderStatus.Open, 1, 1001, DateTime.Today);
            await AddPurchaseOrderAsync(product, PurchaseOrderStatus.Open, 1, 1002, DateTime.Today.AddDays(1));
            var sut = new PurchaseOrderRepository(Context);

            var result = await sut.GetOpenOrdersDueAsync(DateTime.Today);

            Assert.Equal(due.Id, Assert.Single(result).Id);
        }

        [Fact]
        public async Task RemoveLine_ThenUpdate_DeletesTheLine()
        {
            var product = await AddProductAsync(500000142, _ => { });
            var order = await AddPurchaseOrderAsync(product, PurchaseOrderStatus.Open, 1, 1001);
            var sut = new PurchaseOrderRepository(Context);

            var tracked = await sut.GetByNumberAsync(1001);
            sut.RemoveLine(tracked!.Lines.Single());
            await sut.UpdateAsync(tracked);
            Context.ChangeTracker.Clear();

            Assert.Empty((await sut.GetByNumberAsync(1001))!.Lines);
            Assert.Equal(order.Id, tracked.Id);
        }

        [Fact]
        public async Task UpdateAsync_NewLineOnTrackedOrder_InsertsTheLine()
        {
            var product = await AddProductAsync(500000143, _ => { });
            await AddPurchaseOrderAsync(product, PurchaseOrderStatus.Open, 1, 1001);
            var sut = new PurchaseOrderRepository(Context);

            var tracked = await sut.GetByNumberAsync(1001);
            tracked!.Lines.Add(new PurchaseOrderLine { PurchaseOrderId = tracked.Id, ProductId = product.Id, Amount = 4 });
            await sut.UpdateAsync(tracked);
            Context.ChangeTracker.Clear();

            Assert.Equal(2, (await sut.GetByNumberAsync(1001))!.Lines.Count);
        }
    }

    public class CustomerOrderRepositoryTests : OrderingRepositoryTestBase
    {
        [Fact]
        public async Task GetNextOrderNumberAsync_NoOrders_StartsAtFirstCustomerOrderNumber()
        {
            var sut = new CustomerOrderRepository(Context);

            Assert.Equal(IdentifierFormats.FirstCustomerOrderNumber, await sut.GetNextOrderNumberAsync());
        }

        [Fact]
        public async Task GetWaitingForProductsAsync_ReturnsOnlyWaitingOrdersWithTheProduct()
        {
            var product = await AddProductAsync(500000150, _ => { });
            var other = await AddProductAsync(500000151, _ => { });
            var waiting = await AddCustomerOrderAsync(product, CustomerOrderStatus.Waiting, 3, 0, 7000001);
            await AddCustomerOrderAsync(product, CustomerOrderStatus.ReadyForPickup, 3, 3, 7000002);
            await AddCustomerOrderAsync(other, CustomerOrderStatus.Waiting, 3, 0, 7000003);
            var sut = new CustomerOrderRepository(Context);

            var result = await sut.GetWaitingForProductsAsync([product.Id]);

            Assert.Equal(waiting.Id, Assert.Single(result).Id);
        }
    }

    public class StockCountRepositoryTests : OrderingRepositoryTestBase
    {
        [Fact]
        public async Task GetListAsync_OpenCountsComeFirst()
        {
            var product = await AddProductAsync(500000160, _ => { });
            Context.StockCounts.AddRange(
                new StockCount { Id = Guid.NewGuid(), CountNumber = 1001, ShelfId = product.ShelfId, Status = StockCountStatus.Completed },
                new StockCount { Id = Guid.NewGuid(), CountNumber = 1002, ShelfId = product.ShelfId, Status = StockCountStatus.Open });
            await Context.SaveChangesAsync();
            var sut = new StockCountRepository(Context);

            var result = await sut.GetListAsync(null);

            Assert.Equal([1002, 1001], result.Select(c => c.CountNumber));
            Assert.Equal(1003, await sut.GetNextCountNumberAsync());
        }

        [Fact]
        public async Task GetOpenCountForShelfAsync_MatchesTypeAndStatus()
        {
            var product = await AddProductAsync(500000161, _ => { });
            var expiry = new StockCount { Id = Guid.NewGuid(), CountNumber = 1001, ShelfId = product.ShelfId, Type = StockCountType.Expiry };
            Context.StockCounts.Add(expiry);
            await Context.SaveChangesAsync();
            var sut = new StockCountRepository(Context);

            Assert.Equal(expiry.Id, (await sut.GetOpenCountForShelfAsync(product.ShelfId, StockCountType.Expiry))?.Id);
            Assert.Null(await sut.GetOpenCountForShelfAsync(product.ShelfId, StockCountType.Manual));
        }
    }

    public class ProductSalesRepositoryTests : OrderingRepositoryTestBase
    {
        [Fact]
        public async Task GetProductLinesAsync_ReturnsLinesOfThatProductInThePeriod()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000170, _ => { });
            var other = await AddProductAsync(500000171, _ => { });
            Context.CashRegisterLogs.Add(new CashRegisterLogs
            {
                Id = Guid.NewGuid(),
                ReceiptId = "9000000001",
                EmployeeId = employee.Id,
                CreatedAt = DateTime.UtcNow,
                ProductsSold =
                [
                    new RegisterItemDetail { Id = Guid.NewGuid(), ProductId = product.Id, Amount = 3, ReturnedAmount = 1, PriceAtTimeOfSale = 2.5m },
                    new RegisterItemDetail { Id = Guid.NewGuid(), ProductId = other.Id, Amount = 9, PriceAtTimeOfSale = 1m },
                ],
            });
            await Context.SaveChangesAsync();
            var sut = new StatisticsRepository(Context);

            var result = await sut.GetProductLinesAsync(product.Id, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var line = Assert.Single(result);
            Assert.Equal((3, 1, 2.5m), (line.Amount, line.ReturnedAmount, line.Price));
        }
    }
}
