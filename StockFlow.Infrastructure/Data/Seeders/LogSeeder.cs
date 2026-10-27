using System.Globalization;
using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class LogSeeder
    {
        private const int RecalledStock = 6;

        private sealed record StockEvent(Product Product, DateTime At, int Change, Guid EmployeeId, LogType Type, string[] Extra);

        public static void UseLogSeeder(this DbContext context)
        {
            if (context.Set<StockLogs>().Any() || context.Set<CustomerLogs>().Any() || !context.Set<CashRegisterLogs>().Any())
                return;

            var products = context.Set<Product>().Include(p => p.Prices).ToDictionary(p => p.Id);
            var receipts = context.Set<CashRegisterLogs>().Include(r => r.ProductsSold).OrderBy(r => r.CreatedAt).ToList();
            var customers = context.Set<Customer>().ToDictionary(c => c.Id);
            var random = new Random(2027);
            DateTime nowUtc = DateTime.UtcNow;

            var stockLogs = new List<StockLogs>();
            var events = new List<StockEvent>();
            var pointChanges = new List<(Guid CustomerId, DateTime At, int Change, string[] Details)>();
            var returnLogs = new List<ReturnLogs>();

            foreach (var product in products.Values)
            {
                stockLogs.Add(Log(product, nowUtc.AddDays(-120).AddMinutes(-5), SeedIds.Manager, LogType.Create, []));

                var prices = product.Prices.OrderBy(p => p.ValidFrom).ToList();
                var past = prices.Where(p => p.ValidFrom <= DateTime.Today).ToList();
                for (int i = 1; i < past.Count; i++)
                    if (past[i].Price != past[i - 1].Price)
                        stockLogs.Add(Log(product, past[i].CreatedAt, past[i].CreatedById ?? SeedIds.Manager, LogType.Update,
                            [$"Price: New: {Money(past[i].Price)}. Old: {Money(past[i - 1].Price)}"]));

                foreach (var planned in prices.Where(p => p.ValidFrom > DateTime.Today))
                    stockLogs.Add(Log(product, planned.CreatedAt, planned.CreatedById ?? SeedIds.Manager, LogType.Update,
                        [$"ScheduledPrice: {Money(planned.Price)} from {planned.ValidFrom:dd-MM-yyyy}"]));

                if (product.Recalled && product.RecalledAt is DateTime recalledAt)
                {
                    stockLogs.Add(Log(product, recalledAt, SeedIds.Manager, LogType.Recalled, [$"Reason: {product.RecallReason}"]));
                    events.Add(new StockEvent(product, recalledAt.AddSeconds(1), -RecalledStock, SeedIds.Manager, LogType.WrittenOff, ["Reason: Recalled"]));
                }

                if (product.OrderBlocked)
                {
                    string until = product.OrderBlockedUntil is DateTime date ? $" until {date:dd-MM-yyyy}" : "";
                    var blockedAt = product.OrderBlockedUntil is null ? SeedIds.DaysAgo(14, 10) : SeedIds.DaysAgo(3, 11);
                    stockLogs.Add(Log(product, blockedAt, SeedIds.ShiftLeaderOne, LogType.Update, [$"OrderBlock: Blocked{until}", $"Reason: {product.OrderBlockReason}"]));
                }
            }

            foreach (var receipt in receipts)
            {
                foreach (var line in receipt.ProductsSold)
                    if (line.ProductId is Guid productId && products.TryGetValue(productId, out var product))
                        events.Add(new StockEvent(product, receipt.CreatedAt, -line.Amount, receipt.EmployeeId, LogType.Sold, [$"Receipt: {receipt.ReceiptId}"]));

                int earnedAtSale = receipt.PointsEarned;
                if (receipt.CustomerId is Guid buyer && earnedAtSale > 0)
                    pointChanges.Add((buyer, receipt.CreatedAt, earnedAtSale, [$"Receipt: {receipt.ReceiptId}", $"PointsEarned: {earnedAtSale}"]));

                var returned = receipt.ProductsSold.Where(l => l.ReturnedAmount > 0 && l.ProductId is not null).ToList();
                if (returned.Count == 0)
                    continue;

                DateTime returnedAt = receipt.CreatedAt.AddHours(random.Next(2, 48));
                if (returnedAt > nowUtc.AddMinutes(-30))
                    returnedAt = receipt.CreatedAt.AddMinutes(Math.Max(1, (nowUtc.AddMinutes(-30) - receipt.CreatedAt).TotalMinutes / 2));

                decimal linesTotal = receipt.ProductsSold.Sum(l => SalePricing.Total(l.PriceAtTimeOfSale, l.Amount));
                decimal totalPaid = (decimal)receipt.TotalPrice;
                decimal keptValue = receipt.ProductsSold.Sum(l => SalePricing.Total(l.PriceAtTimeOfSale, l.Amount - l.ReturnedAmount))
                    * ReturnRules.PaidShare(totalPaid, linesTotal);
                int stillEarned = Math.Min(receipt.PointsEarned, LoyaltyPoints.PointsEarned(keptValue));
                int deducted = receipt.PointsEarned - stillEarned;
                receipt.PointsEarned = stillEarned;

                var log = new ReturnLogs
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = returnedAt,
                    ReceiptId = receipt.ReceiptId,
                    CashRegisterLogId = receipt.Id,
                    EmployeeId = SeedIds.ServiceDeskOne,
                    CustomerId = receipt.CustomerId,
                    PointsDeducted = deducted,
                    Lines = returned.Select(l => new ReturnLogLine
                    {
                        Id = Guid.NewGuid(),
                        ProductId = l.ProductId!.Value,
                        Amount = l.ReturnedAmount,
                        PriceAtTimeOfSale = l.PriceAtTimeOfSale,
                        RefundAmount = ReturnRules.RefundFor(l.PriceAtTimeOfSale, l.ReturnedAmount, totalPaid, linesTotal),
                    }).ToList(),
                };
                log.RefundAmount = log.Lines.Sum(l => l.RefundAmount);
                returnLogs.Add(log);

                foreach (var line in returned)
                    events.Add(new StockEvent(products[line.ProductId!.Value], returnedAt, line.ReturnedAmount, SeedIds.ServiceDeskOne, LogType.Returned, []));

                if (receipt.CustomerId is Guid returner && deducted > 0)
                    pointChanges.Add((returner, returnedAt, -deducted, [$"Return: Receipt {receipt.ReceiptId}", $"PointsDeducted: {deducted}"]));
            }

            foreach (var order in context.Set<PurchaseOrder>().Include(o => o.Lines).Where(o => o.Status == PurchaseOrderStatus.Received).ToList())
                foreach (var line in order.Lines.Where(l => l.ReceivedAmount > 0))
                    events.Add(new StockEvent(products[line.ProductId], order.ReceivedAt ?? order.UpdatedAt, line.ReceivedAmount,
                        order.ReceivedById ?? SeedIds.FloorWorkerOne, LogType.Received, [$"Order: {order.OrderNumber}"]));

            foreach (var count in context.Set<StockCount>().Include(c => c.Lines).Where(c => c.Status == StockCountStatus.Completed).ToList())
                foreach (var line in count.Lines.Where(l => l.CountedStock is int counted && counted != l.ExpectedStock))
                    events.Add(new StockEvent(products[line.ProductId], count.CompletedAt ?? count.UpdatedAt, line.CountedStock!.Value - line.ExpectedStock,
                        count.CompletedById ?? SeedIds.FloorWorkerTwo, LogType.Counted, [$"Count: {count.CountNumber}"]));

            foreach (var group in events.GroupBy(e => e.Product.Id))
            {
                int stock = group.First().Product.Stock;
                foreach (var change in group.OrderByDescending(e => e.At))
                {
                    int before = stock - change.Change;
                    stockLogs.Add(Log(change.Product, change.At, change.EmployeeId, change.Type, [$"Stock: New: {stock}. Old: {before}", .. change.Extra]));
                    stock = before;
                }
            }

            var customerLogs = new List<CustomerLogs>();
            foreach (var customer in customers.Values)
            {
                var changes = pointChanges.Where(c => c.CustomerId == customer.Id).OrderBy(c => c.At).ToList();
                DateTime createdAt = changes.Count > 0 ? changes[0].At.AddDays(-random.Next(5, 30)) : customer.CreatedAt;
                customer.CreatedAt = createdAt;
                customerLogs.Add(new CustomerLogs { Id = Guid.NewGuid(), CustomerId = customer.Id, CreatedAt = createdAt, LogType = LogType.Create, Messages = [] });

                int points = customer.SavedPoints;
                foreach (var change in changes)
                {
                    int after = Math.Max(0, points + change.Change);
                    customerLogs.Add(new CustomerLogs
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customer.Id,
                        CreatedAt = change.At,
                        LogType = LogType.Update,
                        Messages = [$"SavedPoints: New: {after}. Old: {points}", .. change.Details],
                    });
                    points = after;
                }
                customer.SavedPoints = points;
            }

            context.Set<StockLogs>().AddRange(stockLogs);
            context.Set<CustomerLogs>().AddRange(customerLogs);
            context.Set<ReturnLogs>().AddRange(returnLogs);
            context.SaveChanges();
        }

        private static StockLogs Log(Product product, DateTime at, Guid employeeId, LogType type, string[] messages) => new()
        {
            Id = Guid.NewGuid(),
            CreatedAt = at,
            Identifier = product.Barcode.ToString(),
            StockLogType = StockLogType.Product,
            LogType = type,
            EmployeeId = employeeId,
            Messages = messages,
        };

        private static string Money(decimal value) => value.ToString("0.00", CultureInfo.InvariantCulture);
    }
}
