using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class PurchaseOrderSeeder
    {
        public static void UsePurchaseOrderSeeder(this DbContext context)
        {
            if (context.Set<PurchaseOrder>().Any() || !context.Set<Product>().Any())
                return;

            DateTime today = DateTime.Today;
            DateTime receivedOrderDay = SeedIds.PreviousDay(DayOfWeek.Tuesday, extraWeeks: 1);
            DateTime sentOrderDay = SeedIds.PreviousDay(DayOfWeek.Thursday);
            DateTime cancelledOrderDay = SeedIds.PreviousDay(DayOfWeek.Wednesday, extraWeeks: 2);

            var orders = new List<PurchaseOrder>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = 1001,
                    SupplierId = SupplierSeeder.PaintSupplierId,
                    Status = PurchaseOrderStatus.Received,
                    OrderDate = receivedOrderDay,
                    CreatedAt = Utc(receivedOrderDay.AddDays(-3)),
                    UpdatedAt = Utc(receivedOrderDay.AddDays(2)),
                    SentAt = Utc(receivedOrderDay.AddHours(6)),
                    ReceivedAt = Utc(receivedOrderDay.AddDays(2).AddHours(9)),
                    ReceivedById = SeedIds.FloorWorkerOne,
                    Lines =
                    [
                        Line(500000002, 20, 20, OrderLineSource.Automatic),
                        Line(500000013, 15, 15, OrderLineSource.Manual, SeedIds.ShiftLeaderOne),
                        Line(500000025, 12, 10, OrderLineSource.Manual, SeedIds.ShiftLeaderOne),
                    ],
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = 1002,
                    SupplierId = SupplierSeeder.SanitarySupplierId,
                    Status = PurchaseOrderStatus.Sent,
                    OrderDate = sentOrderDay,
                    CreatedAt = Utc(sentOrderDay.AddDays(-4)),
                    UpdatedAt = Utc(sentOrderDay.AddHours(6)),
                    SentAt = Utc(sentOrderDay.AddHours(6)),
                    Lines =
                    [
                        Line(500000124, 20, 0, OrderLineSource.Manual, SeedIds.ShiftLeaderOne),
                        Line(500000133, 10, 0, OrderLineSource.Manual, SeedIds.Manager),
                        Line(500000142, 8, 0, OrderLineSource.Manual, SeedIds.Manager),
                    ],
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = 1003,
                    SupplierId = Supplier.DefaultSupplierId,
                    Status = PurchaseOrderStatus.Open,
                    OrderDate = OrderDays.NextOrderDate(DayOfWeek.Monday, today),
                    CreatedAt = Utc(today.AddDays(-2)),
                    UpdatedAt = Utc(today.AddDays(-1)),
                    Lines =
                    [
                        Line(500000052, 10, 0, OrderLineSource.Manual, SeedIds.ShiftLeaderOne),
                        Line(500000041, 2, 0, OrderLineSource.CustomerOrder, SeedIds.ServiceDeskOne, SeedIds.WaitingCustomerOrder),
                    ],
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = 1004,
                    SupplierId = SupplierSeeder.ElectroSupplierId,
                    Status = PurchaseOrderStatus.Cancelled,
                    OrderDate = cancelledOrderDay,
                    CreatedAt = Utc(cancelledOrderDay.AddDays(-2)),
                    UpdatedAt = Utc(cancelledOrderDay.AddDays(-1)),
                    Lines = [Line(500000071, 5, 0, OrderLineSource.Manual, SeedIds.Manager)],
                },
            };

            context.Set<PurchaseOrder>().AddRange(orders);
            context.SaveChanges();
        }

        private static PurchaseOrderLine Line(int barcode, int amount, int received, OrderLineSource source, Guid? addedBy = null, Guid? customerOrderId = null) => new()
        {
            Id = Guid.NewGuid(),
            ProductId = SeedIds.Product(barcode),
            Amount = amount,
            ReceivedAmount = received,
            Source = source,
            AddedById = addedBy,
            CustomerOrderId = customerOrderId,
        };

        private static DateTime Utc(DateTime local) => DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime();
    }
}
