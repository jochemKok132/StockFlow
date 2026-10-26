using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class CustomerOrderSeeder
    {
        public static void UseCustomerOrderSeeder(this DbContext context)
        {
            if (context.Set<CustomerOrder>().Any() || !context.Set<Product>().Any())
                return;

            string? pickupReceipt = context.Set<CashRegisterLogs>()
                .Where(l => l.Id == SeedIds.PickupReceipt)
                .Select(l => l.ReceiptId)
                .FirstOrDefault();

            var orders = new List<CustomerOrder>
            {
                new()
                {
                    Id = SeedIds.PickedUpCustomerOrder,
                    OrderNumber = IdentifierFormats.FirstCustomerOrderNumber,
                    CustomerId = SeedIds.Customer(3),
                    Status = pickupReceipt is null ? CustomerOrderStatus.Cancelled : CustomerOrderStatus.PickedUp,
                    Note = "Customer pays at pickup.",
                    CreatedById = SeedIds.ServiceDeskOne,
                    CreatedAt = SeedIds.DaysAgo(8, 10),
                    UpdatedAt = SeedIds.DaysAgo(3, 14, 10),
                    ReadyAt = SeedIds.DaysAgo(8, 10),
                    PickedUpAt = pickupReceipt is null ? null : SeedIds.DaysAgo(3, 14, 10),
                    ReceiptId = pickupReceipt,
                    Lines = [new CustomerOrderLine { ProductId = SeedIds.Product(500000032), Amount = 2 }],
                },
                new()
                {
                    Id = SeedIds.ReadyCustomerOrder,
                    OrderNumber = IdentifierFormats.FirstCustomerOrderNumber + 1,
                    CustomerId = SeedIds.Customer(1),
                    Status = CustomerOrderStatus.ReadyForPickup,
                    Note = "Calls before coming to the store.",
                    CreatedById = SeedIds.ServiceDeskOne,
                    CreatedAt = SeedIds.DaysAgo(1, 11),
                    UpdatedAt = SeedIds.DaysAgo(1, 11),
                    ReadyAt = SeedIds.DaysAgo(1, 11),
                    Lines =
                    [
                        new CustomerOrderLine { ProductId = SeedIds.Product(500000005), Amount = 3, ReservedAmount = 3 },
                        new CustomerOrderLine { ProductId = SeedIds.Product(500000132), Amount = 1, ReservedAmount = 1 },
                    ],
                },
                new()
                {
                    Id = SeedIds.WaitingCustomerOrder,
                    OrderNumber = IdentifierFormats.FirstCustomerOrderNumber + 2,
                    CustomerId = SeedIds.Customer(2),
                    Status = CustomerOrderStatus.Waiting,
                    Note = "Needs 5 for a fence; 2 are on the next order.",
                    CreatedById = SeedIds.ServiceDeskOne,
                    CreatedAt = SeedIds.DaysAgo(1, 15),
                    UpdatedAt = SeedIds.DaysAgo(1, 15),
                    Lines = [new CustomerOrderLine { ProductId = SeedIds.Product(500000041), Amount = 5, ReservedAmount = 3 }],
                },
                new()
                {
                    Id = SeedIds.CancelledCustomerOrder,
                    OrderNumber = IdentifierFormats.FirstCustomerOrderNumber + 3,
                    CustomerId = SeedIds.Customer(4),
                    Status = CustomerOrderStatus.Cancelled,
                    Note = "Customer found it somewhere else.",
                    CreatedById = SeedIds.ShiftLeaderOne,
                    CreatedAt = SeedIds.DaysAgo(9, 16),
                    UpdatedAt = SeedIds.DaysAgo(6, 9),
                    CancelledAt = SeedIds.DaysAgo(6, 9),
                    Lines = [new CustomerOrderLine { ProductId = SeedIds.Product(500000010), Amount = 1 }],
                },
            };

            context.Set<CustomerOrder>().AddRange(orders);
            context.SaveChanges();
        }
    }
}
