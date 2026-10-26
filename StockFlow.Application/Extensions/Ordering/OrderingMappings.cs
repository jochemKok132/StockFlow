using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Mappings.People;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Ordering;
using StockFlow.Shared.DTOs.Stock.Supplier;

namespace StockFlow.Application.Mappings.Ordering
{
    public static class OrderingMappings
    {
        public static SupplierDto ToSupplierDto(this Supplier supplier, DateTime today, int productCount = 0) => new()
        {
            Id = supplier.Id,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt,
            SupplierName = supplier.SupplierName,
            ContactName = supplier.ContactName,
            Email = supplier.Email,
            Phone = supplier.Phone,
            Website = supplier.Website,
            OrderDay = supplier.OrderDay,
            LeadTimeDays = supplier.LeadTimeDays,
            NextOrderDate = OrderDays.NextOrderDate(supplier.OrderDay, today),
            ProductCount = productCount,
        };

        public static Supplier ToSupplierEntity(this CreateSupplierDto dto) => new()
        {
            Id = Guid.NewGuid(),
            SupplierName = dto.SupplierName.Trim(),
            ContactName = Clean(dto.ContactName),
            Email = Clean(dto.Email),
            Phone = Clean(dto.Phone),
            Website = Clean(dto.Website),
            OrderDay = dto.OrderDay,
            LeadTimeDays = dto.LeadTimeDays,
        };

        public static void ToSupplierEntity(this Supplier entity, UpdateSupplierDto dto)
        {
            entity.SupplierName = dto.SupplierName.Trim();
            entity.ContactName = Clean(dto.ContactName);
            entity.Email = Clean(dto.Email);
            entity.Phone = Clean(dto.Phone);
            entity.Website = Clean(dto.Website);
            entity.OrderDay = dto.OrderDay;
            entity.LeadTimeDays = dto.LeadTimeDays;
        }

        public static PurchaseOrderDto ToPurchaseOrderDto(this PurchaseOrder order, IReadOnlyDictionary<Guid, int>? customerOrderNumbers = null) => new()
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            SupplierId = order.SupplierId,
            SupplierName = order.Supplier?.SupplierName ?? "-",
            Status = order.Status.ToDto(),
            OrderDate = order.OrderDate,
            ExpectedDelivery = OrderDays.ExpectedDelivery(order.SentAt?.ToLocalTime().Date ?? order.OrderDate, order.Supplier?.LeadTimeDays ?? 0),
            CreatedAt = order.CreatedAt,
            SentAt = order.SentAt,
            ReceivedAt = order.ReceivedAt,
            ReceivedByName = order.ReceivedBy?.FullName,
            TotalItems = order.Lines.Sum(l => l.Amount),
            Lines = order.Lines
                .OrderBy(l => l.Product?.ProductName)
                .Select(l => new PurchaseOrderLineDto
                {
                    Id = l.Id,
                    ProductId = l.ProductId,
                    Barcode = l.Product?.Barcode ?? 0,
                    ProductName = l.Product?.ProductName ?? "-",
                    CanExpire = l.Product?.CanExpire ?? false,
                    Amount = l.Amount,
                    ReceivedAmount = l.ReceivedAmount,
                    Source = l.Source.ToDto(),
                    CustomerOrderNumber = l.CustomerOrderId is Guid id && customerOrderNumbers is not null && customerOrderNumbers.TryGetValue(id, out var number) ? number : null,
                })
                .ToList(),
        };

        public static CustomerOrderDto ToCustomerOrderDto(this CustomerOrder order) => new()
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToDto(),
            Source = order.Source.ToDto(),
            CustomerId = order.CustomerId,
            CustomerNumber = order.Customer?.CustomerId ?? "-",
            CustomerName = order.Customer?.FullName ?? "-",
            Customer = order.Customer?.ToCustomerDto(),
            Note = order.Note,
            CreatedByName = order.CreatedBy?.FullName,
            CreatedAt = order.CreatedAt,
            ReadyAt = order.ReadyAt,
            PickedUpAt = order.PickedUpAt,
            CancelledAt = order.CancelledAt,
            ReceiptId = order.ReceiptId,
            Lines = order.Lines
                .OrderBy(l => l.Product?.ProductName)
                .Select(l => new CustomerOrderLineDto
                {
                    Id = l.Id,
                    ProductId = l.ProductId,
                    Barcode = l.Product?.Barcode ?? 0,
                    ProductName = l.Product?.ProductName ?? "-",
                    Amount = l.Amount,
                    ReservedAmount = l.ReservedAmount,
                })
                .ToList(),
        };

        public static StockCountDto ToStockCountDto(this StockCount count) => new()
        {
            Id = count.Id,
            CountNumber = count.CountNumber,
            Type = count.Type.ToDto(),
            Status = count.Status.ToDto(),
            ShelfId = count.ShelfId,
            ShelfName = count.Shelf?.ShelfName ?? "-",
            ShelfLocation = count.Shelf?.ShelfLocation ?? "-",
            DueDate = count.DueDate,
            Note = count.Note,
            CreatedAt = count.CreatedAt,
            CreatedByName = count.CreatedBy?.FullName,
            CompletedAt = count.CompletedAt,
            CompletedByName = count.CompletedBy?.FullName,
            Lines = count.Lines
                .OrderBy(l => l.Product?.Location)
                .ThenBy(l => l.Product?.ProductName)
                .Select(l => new StockCountLineDto
                {
                    Id = l.Id,
                    ProductId = l.ProductId,
                    Barcode = l.Product?.Barcode ?? 0,
                    ProductName = l.Product?.ProductName ?? "-",
                    Location = $"{count.Shelf?.ShelfLocation}-{l.Product?.Location}",
                    CanExpire = l.Product?.CanExpire ?? false,
                    FirstExpiryDate = l.Product?.Batches.Where(b => b.Amount > 0).OrderBy(b => b.ExpiryDate).Select(b => (DateTime?)b.ExpiryDate).FirstOrDefault(),
                    ExpectedStock = count.Status == StockCountStatus.Open ? l.Product?.Stock ?? l.ExpectedStock : l.ExpectedStock,
                    CountedStock = l.CountedStock,
                    ExpiredAmount = l.ExpiredAmount,
                    NextExpiryDate = l.NextExpiryDate,
                })
                .ToList(),
        };

        private static string? Clean(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
