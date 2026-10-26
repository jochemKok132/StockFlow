using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.Ordering;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.Ordering;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Mappings.Ordering;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Application.Validations.Stock;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Ordering;

namespace StockFlow.Application.Services.Ordering
{
    public class PurchaseOrderService(IPurchaseOrderRepository orderRepository, IProductRepository productRepository, ISupplierRepository supplierRepository, IStockLogsService stockLogsService, IStockAllocator stockAllocator) : IPurchaseOrderService
    {
        public const string OnlyOpenOrdersChange = "Only an open order can be changed.";
        public const string OrderNotSent = "This order hasn't been sent to the supplier yet.";
        public const string OrderAlreadyReceived = "This order has already been received.";
        public const string OrderCancelled = "This order is cancelled.";

        public async Task<List<PurchaseOrderDto>> GetOrdersAsync(PurchaseOrderStatusDto? status, Guid? supplierId)
        {
            if (status is { } value && !Enum.IsDefined(value))
                throw new ArgumentException("Choose a valid status.");
            var orders = await orderRepository.GetListAsync(status?.ToDomain(), supplierId);
            var numbers = await CustomerOrderNumbersAsync(orders.SelectMany(o => o.Lines));
            return orders.Select(o => o.ToPurchaseOrderDto(numbers)).ToList();
        }

        public async Task<PurchaseOrderDto> GetByNumberAsync(int orderNumber)
        {
            var order = await GetOrderAsync(orderNumber);
            return order.ToPurchaseOrderDto(await CustomerOrderNumbersAsync(order.Lines));
        }

        public async Task<PurchaseOrderDto> AddLineAsync(AddOrderLineRequest request, Guid employeeId)
        {
            request.Validate();
            DateTime today = DateTime.Today;
            var product = await productRepository.GetProductByBarcodeAsync(request.Barcode);
            if (product is null || product.SoftDeleted)
                throw new KeyNotFoundException($"Product with the barcode {request.Barcode} not found.");

            string? refusal = product.OrderRefusal(today);
            if (refusal is not null)
                throw new ArgumentException(refusal);

            var order = await AddToOpenOrderAsync(product, request.Amount, OrderLineSource.Manual, null, employeeId, today);
            return await GetByNumberAsync(order.OrderNumber);
        }

        public async Task<PurchaseOrder> AddToOpenOrderAsync(Product product, int amount, OrderLineSource source, Guid? customerOrderId, Guid? employeeId, DateTime today)
        {
            var order = await orderRepository.GetOpenForSupplierAsync(product.SupplierId);
            if (order is null)
            {
                var supplier = await supplierRepository.GetByIdAsync(product.SupplierId);
                if (supplier is null || supplier.SoftDeleted)
                    throw new ArgumentException($"{product.ProductName} has no active supplier.");

                order = new PurchaseOrder
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = await orderRepository.GetNextOrderNumberAsync(),
                    SupplierId = supplier.Id,
                    Status = PurchaseOrderStatus.Open,
                    OrderDate = OrderDays.NextOrderDate(supplier.OrderDay, today),
                };
                await orderRepository.AddAsync(order);
            }

            var existing = source == OrderLineSource.CustomerOrder
                ? null
                : order.Lines.FirstOrDefault(l => l.ProductId == product.Id && l.Source == source && l.CustomerOrderId is null);
            if (existing is not null)
            {
                existing.Amount = Math.Min(FieldLimits.MaxOrderAmount, existing.Amount + amount);
            }
            else
            {
                order.Lines.Add(new PurchaseOrderLine
                {
                                    PurchaseOrderId = order.Id,
                    ProductId = product.Id,
                    Amount = Math.Min(FieldLimits.MaxOrderAmount, amount),
                    Source = source,
                    CustomerOrderId = customerOrderId,
                    AddedById = employeeId,
                });
            }

            await orderRepository.UpdateAsync(order);
            return order;
        }

        public async Task<PurchaseOrderDto> UpdateLineAsync(int orderNumber, UpdateOrderLineRequest request)
        {
            request.Validate();
            var order = await GetOrderAsync(orderNumber);
            if (order.Status != PurchaseOrderStatus.Open)
                throw new ArgumentException(OnlyOpenOrdersChange);

            var line = order.Lines.FirstOrDefault(l => l.Id == request.LineId)
                ?? throw new KeyNotFoundException("Order line not found.");
            if (line.Source == OrderLineSource.CustomerOrder)
                throw new ArgumentException("This line is for a customer order. Cancel the customer order to remove it.");

            if (request.Amount == 0)
                orderRepository.RemoveLine(line);
            else
                line.Amount = request.Amount;

            await orderRepository.UpdateAsync(order);
            return await GetByNumberAsync(orderNumber);
        }

        public async Task<PurchaseOrderDto> CancelAsync(int orderNumber)
        {
            var order = await GetOrderAsync(orderNumber);
            if (order.Status != PurchaseOrderStatus.Open)
                throw new ArgumentException("Only an open order can be cancelled.");
            if (order.Lines.Any(l => l.Source == OrderLineSource.CustomerOrder))
                throw new ArgumentException("This order contains products for customer orders. Cancel those customer orders first.");

            order.Status = PurchaseOrderStatus.Cancelled;
            await orderRepository.UpdateAsync(order);
            return await GetByNumberAsync(orderNumber);
        }

        public async Task<PurchaseOrderDto> ReceiveAsync(int orderNumber, ReceiveOrderRequest request, Guid employeeId)
        {
            request.Validate();
            DateTime today = DateTime.Today;
            var order = await GetOrderAsync(orderNumber);
            switch (order.Status)
            {
                case PurchaseOrderStatus.Open: throw new ArgumentException(OrderNotSent);
                case PurchaseOrderStatus.Received: throw new ArgumentException(OrderAlreadyReceived);
                case PurchaseOrderStatus.Cancelled: throw new ArgumentException(OrderCancelled);
            }

            var deliveries = new List<(PurchaseOrderLine Line, Product Product, int Amount, DateTime? Expiry)>();
            foreach (var entry in request.Lines)
            {
                var line = order.Lines.FirstOrDefault(l => l.Id == entry.LineId)
                    ?? throw new KeyNotFoundException("Order line not found.");
                var product = await productRepository.GetByIdAsync(line.ProductId)
                    ?? throw new KeyNotFoundException("Product not found.");

                if (entry.ReceivedAmount > 0 && product.CanExpire)
                {
                    if (entry.ExpiryDate is null)
                        throw new ArgumentException($"Enter the expiry date of {product.ProductName}.");
                    if (entry.ExpiryDate.Value.Date <= today)
                        throw new ArgumentException($"The expiry date of {product.ProductName} must be in the future.");
                }
                deliveries.Add((line, product, entry.ReceivedAmount, entry.ExpiryDate?.Date));
            }

            foreach (var (line, product, amount, expiry) in deliveries)
            {
                line.ReceivedAmount = amount;
                if (amount <= 0) continue;

                var log = product.ChangeStock(amount, employeeId, LogTypeDto.Received);
                log.Messages.Add($"Order: {order.OrderNumber}");
                product.ChangeStock(amount);
                if (product.CanExpire && expiry is DateTime date)
                    product.Batches.Add(new StockBatch { ProductId = product.Id, Amount = amount, ExpiryDate = date, CreatedAt = DateTime.UtcNow });

                await productRepository.UpdateAsync(product);
                await stockLogsService.CreateStockLogsAsync(log);
            }

            order.Status = PurchaseOrderStatus.Received;
            order.ReceivedAt = DateTime.UtcNow;
            order.ReceivedById = employeeId;
            await orderRepository.UpdateAsync(order);

            await stockAllocator.AllocateAsync(deliveries.Where(d => d.Amount > 0).Select(d => d.Product.Id));
            return await GetByNumberAsync(orderNumber);
        }

        public async Task<List<ReorderSuggestionDto>> GetReorderSuggestionsAsync()
        {
            DateTime today = DateTime.Today;
            var products = await productRepository.GetMinimumStockProductsAsync(autoOrderOnly: false);
            var ids = products.Select(p => p.Id).ToList();
            var reserved = await productRepository.GetReservedStockAsync(ids);
            var onOrder = await productRepository.GetOnOrderAsync(ids);

            return products
                .Where(p => p.OrderRefusal(today) is null)
                .Select(p => new ReorderSuggestionDto
                {
                    ProductId = p.Id,
                    Barcode = p.Barcode,
                    ProductName = p.ProductName,
                    SupplierName = p.Supplier?.SupplierName ?? "-",
                    Stock = p.Stock,
                    ReservedStock = reserved.GetValueOrDefault(p.Id),
                    OnOrder = onOrder.GetValueOrDefault(p.Id),
                    MinimumStock = p.MinimumStock,
                    MaximumStock = p.MaximumStock,
                    AutoOrder = p.AutoOrder,
                    SuggestedAmount = Reorder.SuggestedAmount(p.Stock, reserved.GetValueOrDefault(p.Id), onOrder.GetValueOrDefault(p.Id), p.MinimumStock, p.MaximumStock),
                })
                .Where(s => s.SuggestedAmount > 0)
                .OrderBy(s => s.SupplierName)
                .ThenBy(s => s.ProductName)
                .ToList();
        }

        public async Task<int> CreateAutomaticOrdersAsync(DateTime today)
        {
            var products = await productRepository.GetMinimumStockProductsAsync(autoOrderOnly: true);
            var ids = products.Select(p => p.Id).ToList();
            var reserved = await productRepository.GetReservedStockAsync(ids);
            var onOrder = await productRepository.GetOnOrderAsync(ids);

            int lines = 0;
            foreach (var product in products)
            {
                if (product.OrderRefusal(today) is not null) continue;
                int amount = Reorder.SuggestedAmount(product.Stock, reserved.GetValueOrDefault(product.Id), onOrder.GetValueOrDefault(product.Id), product.MinimumStock, product.MaximumStock);
                if (amount <= 0) continue;

                await AddToOpenOrderAsync(product, amount, OrderLineSource.Automatic, null, null, today);
                lines++;
            }
            return lines;
        }

        public async Task<int> SendDueOrdersAsync(DateTime today)
        {
            int sent = 0;
            foreach (var order in await orderRepository.GetOpenOrdersDueAsync(today))
            {
                if (order.Lines.Count == 0)
                {
                    order.OrderDate = OrderDays.NextOrderDate(order.Supplier?.OrderDay ?? order.OrderDate.DayOfWeek, today);
                }
                else
                {
                    order.Status = PurchaseOrderStatus.Sent;
                    order.SentAt = DateTime.UtcNow;
                    sent++;
                }
                await orderRepository.UpdateAsync(order);
            }
            return sent;
        }

        public async Task RescheduleOpenOrdersAsync(Guid supplierId, DayOfWeek orderDay, DateTime today)
        {
            foreach (var order in await orderRepository.GetOpenOrdersForSupplierAsync(supplierId))
            {
                order.OrderDate = OrderDays.NextOrderDate(orderDay, today);
                await orderRepository.UpdateAsync(order);
            }
        }

        public async Task RemoveOpenCustomerOrderLinesAsync(Guid customerOrderId)
        {
            var lines = await orderRepository.GetOpenLinesForCustomerOrderAsync(customerOrderId);
            if (lines.Count == 0) return;
            foreach (var line in lines)
                orderRepository.RemoveLine(line);
            await orderRepository.SaveChangesAsync();
        }

        private async Task<PurchaseOrder> GetOrderAsync(int orderNumber) =>
            await orderRepository.GetByNumberAsync(orderNumber)
            ?? throw new KeyNotFoundException($"Order {orderNumber} not found.");

        private async Task<IReadOnlyDictionary<Guid, int>> CustomerOrderNumbersAsync(IEnumerable<PurchaseOrderLine> lines)
        {
            var ids = lines.Where(l => l.CustomerOrderId is not null).Select(l => l.CustomerOrderId!.Value).Distinct().ToList();
            return ids.Count == 0 ? new Dictionary<Guid, int>() : await orderRepository.GetCustomerOrderNumbersAsync(ids);
        }
    }
}
