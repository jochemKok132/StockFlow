using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Ordering;
using StockFlow.Application.Interfaces.Repositories.ManagementApp;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.Ordering;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Mappings.Ordering;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Application.Validations.Stock;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Ordering;

namespace StockFlow.Application.Services.Ordering
{
    public class CustomerOrderService(
        ICustomerOrderRepository customerOrderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IProductService productService,
        IPurchaseOrderService purchaseOrderService,
        IStockAllocator stockAllocator) : ICustomerOrderService
    {
        public async Task<List<CustomerOrderDto>> GetOrdersAsync(CustomerOrderStatusDto? status)
        {
            if (status is { } value && !Enum.IsDefined(value))
                throw new ArgumentException("Choose a valid status.");
            var orders = await customerOrderRepository.GetListAsync(status?.ToDomain());
            return orders.Select(o => o.ToCustomerOrderDto()).ToList();
        }

        public async Task<CustomerOrderDto> GetByNumberAsync(int orderNumber) =>
            (await GetOrderAsync(orderNumber)).ToCustomerOrderDto();

        public async Task<CustomerOrderDto> CreateAsync(CreateCustomerOrderRequest request, Guid employeeId)
        {
            request.Validate();
            DateTime today = DateTime.Today;

            var customer = await customerRepository.GetCustomerByCustomerIdAsync(request.CustomerId.Trim());
            if (customer is null || customer.SoftDeleted)
                throw new KeyNotFoundException($"Customer with customer id {request.CustomerId.Trim()} not found.");

            var requested = request.Lines
                .GroupBy(l => l.Barcode)
                .Select(g => (Barcode: g.Key, Amount: g.Sum(l => l.Amount)))
                .ToList();
            if (requested.Any(r => r.Amount > FieldLimits.MaxOrderAmount))
                throw new ArgumentException($"The amount per product can be at most {FieldLimits.MaxOrderAmount}.");

            var products = new List<(Product Product, int Amount)>();
            foreach (var (barcode, amount) in requested)
            {
                var product = await productRepository.GetProductByBarcodeAsync(barcode);
                if (product is null || product.SoftDeleted)
                    throw new KeyNotFoundException($"Product with the barcode {barcode} not found.");
                string? refusal = product.SaleRefusal();
                if (refusal is not null)
                    throw new ArgumentException(refusal);
                products.Add((product, amount));
            }

            var reserved = await productRepository.GetReservedStockAsync(products.Select(p => p.Product.Id));
            var plan = new List<(Product Product, int Amount, int Reserve, int Shortfall)>();
            foreach (var (product, amount) in products)
            {
                int available = Math.Max(0, product.Stock - reserved.GetValueOrDefault(product.Id));
                int reserve = Math.Min(available, amount);
                int shortfall = amount - reserve;
                if (shortfall > 0 && product.OrderRefusal(today) is string refusal)
                    throw new ArgumentException($"Only {available} of {product.ProductName} available. {refusal}");
                plan.Add((product, amount, reserve, shortfall));
            }

            bool ready = plan.All(p => p.Shortfall == 0);
            var order = new CustomerOrder
            {
                Id = Guid.NewGuid(),
                OrderNumber = await customerOrderRepository.GetNextOrderNumberAsync(),
                CustomerId = customer.Id,
                Status = ready ? CustomerOrderStatus.ReadyForPickup : CustomerOrderStatus.Waiting,
                Source = CustomerOrderSource.Manual,
                Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
                CreatedById = employeeId,
                ReadyAt = ready ? DateTime.UtcNow : null,
                Lines = plan.Select(p => new CustomerOrderLine
                {
                    ProductId = p.Product.Id,
                    Amount = p.Amount,
                    ReservedAmount = p.Reserve,
                }).ToList(),
            };
            await customerOrderRepository.AddAsync(order);

            foreach (var item in plan.Where(p => p.Shortfall > 0))
                await purchaseOrderService.AddToOpenOrderAsync(item.Product, item.Shortfall, OrderLineSource.CustomerOrder, order.Id, employeeId, today);

            return await GetByNumberAsync(order.OrderNumber);
        }

        public async Task<CustomerOrderDto> CancelAsync(int orderNumber)
        {
            var order = await GetOrderAsync(orderNumber);
            if (order.Status is CustomerOrderStatus.PickedUp or CustomerOrderStatus.Cancelled)
                throw new ArgumentException(Describe(order));

            order.Status = CustomerOrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;
            foreach (var line in order.Lines)
                line.ReservedAmount = 0;
            await customerOrderRepository.UpdateAsync(order);

            await purchaseOrderService.RemoveOpenCustomerOrderLinesAsync(order.Id);
            await stockAllocator.AllocateAsync(order.Lines.Select(l => l.ProductId));
            return await GetByNumberAsync(orderNumber);
        }

        public async Task<CustomerOrderDto> GetForPickupAsync(int orderNumber)
        {
            var order = await GetOrderAsync(orderNumber);
            EnsureReady(order);

            var dto = order.ToCustomerOrderDto();
            foreach (var line in dto.Lines)
                line.Product = await productService.GetSellableProductByBarcodeAsync(line.Barcode);
            return dto;
        }

        public async Task ValidatePickupAsync(Purchase purchase)
        {
            if (purchase.CustomerOrderNumber is not int number) return;

            var order = await GetOrderAsync(number);
            EnsureReady(order);
            if (purchase.CustomerId != order.CustomerId)
                throw new ArgumentException($"Customer order {number} belongs to another customer.");

            var bought = purchase.ProductsSold
                .GroupBy(l => l.Product.Id)
                .ToDictionary(g => g.Key, g => g.Sum(l => l.Amount));
            foreach (var line in order.Lines)
            {
                if (bought.GetValueOrDefault(line.ProductId) < line.Amount)
                    throw new ArgumentException($"The receipt is missing products of customer order {number}.");
            }
        }

        public async Task MarkPickedUpAsync(int orderNumber, string receiptId)
        {
            var order = await GetOrderAsync(orderNumber);
            order.Status = CustomerOrderStatus.PickedUp;
            order.PickedUpAt = DateTime.UtcNow;
            order.ReceiptId = receiptId;
            foreach (var line in order.Lines)
                line.ReservedAmount = 0;
            await customerOrderRepository.UpdateAsync(order);
        }

        private static void EnsureReady(CustomerOrder order)
        {
            if (order.Status != CustomerOrderStatus.ReadyForPickup)
                throw new ArgumentException(Describe(order));
        }

        private static string Describe(CustomerOrder order) => order.Status switch
        {
            CustomerOrderStatus.Waiting => $"Customer order {order.OrderNumber} is still waiting for stock and can't be picked up yet.",
            CustomerOrderStatus.PickedUp => $"Customer order {order.OrderNumber} has already been picked up (receipt {order.ReceiptId}).",
            CustomerOrderStatus.Cancelled => $"Customer order {order.OrderNumber} is cancelled.",
            _ => $"Customer order {order.OrderNumber} is ready for pickup.",
        };

        private async Task<CustomerOrder> GetOrderAsync(int orderNumber) =>
            await customerOrderRepository.GetByNumberAsync(orderNumber)
            ?? throw new KeyNotFoundException($"Customer order {orderNumber} not found.");
    }
}
