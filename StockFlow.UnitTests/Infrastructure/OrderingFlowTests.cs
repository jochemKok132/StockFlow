using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Services.ManagementApp.Stock;
using StockFlow.Application.Services.Ordering;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Infrastructure.Repositories;
using StockFlow.Infrastructure.Repositories.Ordering;
using StockFlow.Infrastructure.Repositories.Stock;
using StockFlow.Shared.DTOs.Ordering;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Infrastructure
{
    public class OrderingFlowTests : OrderingRepositoryTestBase
    {
        private readonly Mock<IStockLogsService> _stockLogs = new();
        private readonly ProductRepository _products;
        private readonly SupplierRepository _suppliers;
        private readonly PurchaseOrderRepository _purchaseOrders;
        private readonly CustomerOrderRepository _customerOrders;
        private readonly StockCountRepository _stockCounts;
        private readonly ProductService _productService;
        private readonly PurchaseOrderService _purchaseOrderService;
        private readonly CustomerOrderService _customerOrderService;
        private readonly StockCountService _stockCountService;
        private readonly ProductControlService _productControlService;

        public OrderingFlowTests()
        {
            _products = new ProductRepository(Context);
            _suppliers = new SupplierRepository(Context);
            _purchaseOrders = new PurchaseOrderRepository(Context);
            _customerOrders = new CustomerOrderRepository(Context);
            _stockCounts = new StockCountRepository(Context);
            var allocator = new StockAllocator(_customerOrders, _products);
            _productService = new ProductService(_products, new SalesRepository(Context), new ShelfRepository(Context), new ProductBrandRepository(Context), _stockLogs.Object, _suppliers);
            _purchaseOrderService = new PurchaseOrderService(_purchaseOrders, _products, _suppliers, _stockLogs.Object, allocator);
            _customerOrderService = new CustomerOrderService(_customerOrders, new CustomerRepository(Context), _products, _productService, _purchaseOrderService, allocator);
            _stockCountService = new StockCountService(_stockCounts, _products, new ShelfRepository(Context), _stockLogs.Object, allocator);
            _productControlService = new ProductControlService(_products, _purchaseOrders, _stockLogs.Object, _productService);
        }

        [Fact]
        public async Task UpdateProductAsync_NewPrice_StoresPriceHistory()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000201, p => p.Prices.Add(new ProductPrice { Price = 10m, ValidFrom = DateTime.Today.AddDays(-10) }));
            var dto = TestData.UpdateProductDto(product);
            dto.Price = 12.50m;

            await _productService.UpdateProductAsync(dto, employee.Id);
            Context.ChangeTracker.Clear();

            var prices = await Context.ProductPrices.Where(p => p.ProductId == product.Id).ToListAsync();
            Assert.Equal(2, prices.Count);
            Assert.Contains(prices, p => p.Price == 12.50m && p.ValidFrom == DateTime.Today && p.CreatedById == employee.Id);
        }

        [Fact]
        public async Task SchedulePriceAsync_FuturePrice_AppliesOnItsDayAndCanBeCancelledBefore()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000202, p => p.Price = 10m);

            var planned = await _productControlService.SchedulePriceAsync(new SchedulePriceRequest { ProductId = product.Id, Price = 8m, ValidFrom = DateTime.Today.AddDays(3) }, employee.Id);
            Assert.Equal(10m, planned.Price);
            Assert.True(Assert.Single(planned.Prices).IsFuture);

            Assert.Equal(0, await _productControlService.ApplyDuePricesAsync(DateTime.Today));
            Assert.Equal(1, await _productControlService.ApplyDuePricesAsync(DateTime.Today.AddDays(3)));
            Context.ChangeTracker.Clear();
            Assert.Equal(8m, (await Context.Products.SingleAsync(p => p.Id == product.Id)).Price);
        }

        [Fact]
        public async Task CancelScheduledPriceAsync_FuturePrice_RemovesIt()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000203, _ => { });
            var planned = await _productControlService.SchedulePriceAsync(new SchedulePriceRequest { ProductId = product.Id, Price = 8m, ValidFrom = DateTime.Today.AddDays(3) }, employee.Id);

            var result = await _productControlService.CancelScheduledPriceAsync(product.Id, planned.Prices.Single().Id, employee.Id);

            Assert.Empty(result.Prices);
            Context.ChangeTracker.Clear();
            Assert.False(await Context.ProductPrices.AnyAsync());
        }

        [Fact]
        public async Task AddLineAsync_SameProductTwice_AddsUpOnTheSuppliersOpenOrder()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000204, p => p.Supplier!.OrderDay = DayOfWeek.Wednesday);

            await _purchaseOrderService.AddLineAsync(new AddOrderLineRequest { Barcode = product.Barcode, Amount = 3 }, employee.Id);
            var order = await _purchaseOrderService.AddLineAsync(new AddOrderLineRequest { Barcode = product.Barcode, Amount = 2 }, employee.Id);

            Assert.Equal(5, Assert.Single(order.Lines).Amount);
            Assert.Equal(OrderDays.NextOrderDate(DayOfWeek.Wednesday, DateTime.Today), order.OrderDate);
            Assert.Equal(PurchaseOrderStatusDto.Open, order.Status);
        }

        [Fact]
        public async Task AddLineAsync_NonOrderableProduct_ThrowsArgumentException()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000205, p => p.Orderable = false);

            await Assert.ThrowsAsync<ArgumentException>(() => _purchaseOrderService.AddLineAsync(new AddOrderLineRequest { Barcode = product.Barcode, Amount = 1 }, employee.Id));
        }

        [Fact]
        public async Task SendThenReceive_ExpirableProduct_RaisesStockAddsBatchAndReadiesWaitingCustomerOrder()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000206, p => { p.Stock = 0; p.CanExpire = true; });
            var customer = await AddCustomerAsync();

            var customerOrder = await _customerOrderService.CreateAsync(new CreateCustomerOrderRequest
            {
                CustomerId = customer.CustomerId,
                Lines = [new CustomerOrderLineRequest { Barcode = product.Barcode, Amount = 2 }],
            }, employee.Id);
            Assert.Equal(CustomerOrderStatusDto.Waiting, customerOrder.Status);

            var open = (await _purchaseOrderService.GetOrdersAsync(PurchaseOrderStatusDto.Open, null)).Single();
            var line = Assert.Single(open.Lines);
            Assert.Equal(OrderLineSourceDto.CustomerOrder, line.Source);
            Assert.Equal(customerOrder.OrderNumber, line.CustomerOrderNumber);

            Assert.Equal(1, await _purchaseOrderService.SendDueOrdersAsync(open.OrderDate));
            var received = await _purchaseOrderService.ReceiveAsync(open.OrderNumber, new ReceiveOrderRequest
            {
                Lines = [new ReceiveOrderLine { LineId = line.Id, ReceivedAmount = 5, ExpiryDate = DateTime.Today.AddDays(40) }],
            }, employee.Id);

            Assert.Equal(PurchaseOrderStatusDto.Received, received.Status);
            Context.ChangeTracker.Clear();
            var stored = await Context.Products.Include(p => p.Batches).SingleAsync(p => p.Id == product.Id);
            Assert.Equal(5, stored.Stock);
            Assert.Equal(5, Assert.Single(stored.Batches).Amount);
            var ready = await _customerOrderService.GetByNumberAsync(customerOrder.OrderNumber);
            Assert.Equal(CustomerOrderStatusDto.ReadyForPickup, ready.Status);
            Assert.Equal(2, ready.Lines.Single().ReservedAmount);
        }

        [Fact]
        public async Task ReceiveAsync_ExpirableWithoutExpiryDate_ThrowsArgumentException()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000207, p => p.CanExpire = true);
            var order = await AddPurchaseOrderAsync(product, PurchaseOrderStatus.Sent, 3);

            await Assert.ThrowsAsync<ArgumentException>(() => _purchaseOrderService.ReceiveAsync(order.OrderNumber, new ReceiveOrderRequest
            {
                Lines = [new ReceiveOrderLine { LineId = order.Lines.Single().Id, ReceivedAmount = 3 }],
            }, employee.Id));
        }

        [Fact]
        public async Task CancelAsync_CustomerOrder_RemovesItsOpenSupplierOrderLines()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000208, p => p.Stock = 1);
            var customer = await AddCustomerAsync();
            var order = await _customerOrderService.CreateAsync(new CreateCustomerOrderRequest
            {
                CustomerId = customer.CustomerId,
                Lines = [new CustomerOrderLineRequest { Barcode = product.Barcode, Amount = 4 }],
            }, employee.Id);
            Assert.Equal(1, order.Lines.Single().ReservedAmount);

            var cancelled = await _customerOrderService.CancelAsync(order.OrderNumber);

            Assert.Equal(CustomerOrderStatusDto.Cancelled, cancelled.Status);
            Assert.Empty((await _purchaseOrderService.GetOrdersAsync(PurchaseOrderStatusDto.Open, null)).Single().Lines);
        }

        [Fact]
        public async Task CreateAutomaticOrdersAsync_AutoOrderBelowMinimum_OrdersUpToMaximumOnce()
        {
            await AddProductAsync(500000209, p => { p.Stock = 2; p.MinimumStock = 5; p.MaximumStock = 20; p.AutoOrder = true; });

            Assert.Equal(1, await _purchaseOrderService.CreateAutomaticOrdersAsync(DateTime.Today));
            Assert.Equal(0, await _purchaseOrderService.CreateAutomaticOrdersAsync(DateTime.Today));

            var line = (await _purchaseOrderService.GetOrdersAsync(null, null)).Single().Lines.Single();
            Assert.Equal(18, line.Amount);
            Assert.Equal(OrderLineSourceDto.Automatic, line.Source);
        }

        [Fact]
        public async Task CompleteAsync_ExpiryCount_WritesOffExpiredSetsStockAndResetsBatches()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000210, p =>
            {
                p.Stock = 10;
                p.CanExpire = true;
                p.Batches.Add(new StockBatch { Amount = 4, ExpiryDate = DateTime.Today.AddDays(3) });
                p.Batches.Add(new StockBatch { Amount = 6, ExpiryDate = DateTime.Today.AddDays(60) });
            });

            Assert.Equal(1, await _stockCountService.CreateExpiryCountsAsync(DateTime.Today));
            Assert.Equal(0, await _stockCountService.CreateExpiryCountsAsync(DateTime.Today));
            var count = (await _stockCountService.GetCountsAsync(StockCountStatusDto.Open)).Single();
            Assert.Equal(StockCountTypeDto.Expiry, count.Type);
            Assert.Equal(DateTime.Today.AddDays(2), count.DueDate);
            await Assert.ThrowsAsync<ArgumentException>(() => _stockCountService.CancelAsync(count.CountNumber));

            var done = await _stockCountService.CompleteAsync(count.CountNumber, new CompleteStockCountRequest
            {
                Lines = [new StockCountEntry { LineId = count.Lines.Single().Id, CountedStock = 5, ExpiredAmount = 3, NextExpiryDate = DateTime.Today.AddDays(60) }],
            }, employee.Id);

            Assert.Equal(StockCountStatusDto.Completed, done.Status);
            Assert.Equal(10, done.Lines.Single().ExpectedStock);
            Context.ChangeTracker.Clear();
            var stored = await Context.Products.Include(p => p.Batches).SingleAsync(p => p.Id == product.Id);
            Assert.Equal(5, stored.Stock);
            var batch = Assert.Single(stored.Batches);
            Assert.Equal((5, DateTime.Today.AddDays(60)), (batch.Amount, batch.ExpiryDate));
            _stockLogs.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.LogType == LogTypeDto.WrittenOff)), Times.Once);
            _stockLogs.Verify(s => s.CreateStockLogsAsync(It.Is<CreateStockLogsDto>(l => l.LogType == LogTypeDto.Counted)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ManualCount_ListsTheShelfsProducts()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000211, _ => { });

            var count = await _stockCountService.CreateAsync(new CreateStockCountRequest { ShelfId = product.ShelfId }, employee.Id);

            Assert.Equal(product.Id, Assert.Single(count.Lines).ProductId);
            await Assert.ThrowsAsync<ArgumentException>(() => _stockCountService.CreateAsync(new CreateStockCountRequest { ShelfId = product.ShelfId }, employee.Id));
        }

        [Fact]
        public async Task WriteOffStockAsync_ExpirableProduct_ConsumesTheEarliestBatchFirst()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000212, p =>
            {
                p.Stock = 5;
                p.CanExpire = true;
                p.Batches.Add(new StockBatch { Amount = 2, ExpiryDate = DateTime.Today.AddDays(10) });
                p.Batches.Add(new StockBatch { Amount = 3, ExpiryDate = DateTime.Today.AddDays(20) });
            });

            await _productService.WriteOffStockAsync(new WriteOffRequest { ProductId = product.Id, Amount = 3, Reason = WriteOffReasonDto.Damaged }, employee.Id);
            Context.ChangeTracker.Clear();

            var batch = Assert.Single(await Context.StockBatches.Where(b => b.ProductId == product.Id).ToListAsync());
            Assert.Equal((2, DateTime.Today.AddDays(20)), (batch.Amount, batch.ExpiryDate));
        }

        [Fact]
        public async Task RecallAsync_WithWriteOff_ClearsStockAndRemovesOpenOrderLines()
        {
            var employee = await AddEmployeeAsync();
            var product = await AddProductAsync(500000213, p => p.Stock = 7);
            await AddPurchaseOrderAsync(product, PurchaseOrderStatus.Open, 3);

            var result = await _productControlService.RecallAsync(new RecallRequest { ProductId = product.Id, Reason = "Faulty batch", WriteOffStock = true }, employee.Id);

            Assert.True(result.Recalled);
            Assert.Equal(0, result.Stock);
            Context.ChangeTracker.Clear();
            Assert.False(await Context.PurchaseOrderLines.AnyAsync());
            await Assert.ThrowsAsync<ArgumentException>(() => _productService.GetSellableProductByBarcodeAsync(product.Barcode));
        }
    }
}
