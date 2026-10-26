using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.CashRegister;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class CashRegisterLogSeeder
    {
        private const int Purchases = 80;
        private const int Days = 30;

        public static void UseCashRegisterLogSeeder(this DbContext context)
        {
            if (context.Set<CashRegisterLogs>().Any())
                return;

            var products = context.Set<Product>()
                .Where(p => p.Status == ProductStatus.Active && !p.Recalled)
                .OrderBy(p => p.Barcode)
                .ToList();
            if (products.Count == 0)
                return;

            var random = new Random(2026);
            Guid[] cashiers = [SeedIds.CashierOne, SeedIds.CashierTwo, SeedIds.ServiceDeskOne];
            var logs = new List<CashRegisterLogs>();
            long receiptId = IdentifierFormats.FirstReceiptId;

            var pickup = products.First(p => p.Barcode == 500000032);
            logs.Add(Log(receiptId++, SeedIds.DaysAgo(3, 14, 10), SeedIds.CashierOne, SeedIds.Customer(3), [(pickup, 2, 0)], SeedIds.PickupReceipt));

            for (int i = 0; i < Purchases; i++)
            {
                int daysAgo = random.Next(0, Days);
                var lines = new List<(Product Product, int Amount, int Returned)>();
                int lineCount = random.Next(1, 4);
                for (int l = 0; l < lineCount; l++)
                {
                    var product = products[random.Next(products.Count)];
                    if (lines.Any(x => x.Product.Id == product.Id)) continue;
                    int amount = random.Next(1, 4);
                    int returned = random.Next(20) == 0 ? 1 : 0;
                    lines.Add((product, amount, returned));
                }
                Guid? customer = random.Next(10) < 4 ? SeedIds.Customer(random.Next(1, 16)) : null;
                logs.Add(Log(receiptId++, SeedIds.DaysAgo(daysAgo, random.Next(8, 20), random.Next(60)), cashiers[random.Next(cashiers.Length)], customer, lines));
            }

            context.Set<CashRegisterLogs>().AddRange(logs.OrderBy(l => l.CreatedAt).Select((log, index) =>
            {
                log.ReceiptId = (IdentifierFormats.FirstReceiptId + index).ToString();
                return log;
            }).ToList());
            context.SaveChanges();
        }

        private static CashRegisterLogs Log(long receiptId, DateTime createdAt, Guid employeeId, Guid? customerId, List<(Product Product, int Amount, int Returned)> lines, Guid? fixedId = null)
        {
            decimal total = lines.Sum(l => SalePricing.Total(l.Product.Price, l.Amount));
            var id = fixedId ?? Guid.NewGuid();
            return new CashRegisterLogs
            {
                Id = id,
                ReceiptId = receiptId.ToString(),
                CreatedAt = createdAt,
                EmployeeId = employeeId,
                CustomerId = customerId,
                TotalPrice = (double)total,
                TotalOff = 0,
                PointsEarned = customerId is null ? 0 : LoyaltyPoints.PointsEarned(total),
                ProductsSold = lines.Select(l => new RegisterItemDetail
                {
                    Id = Guid.NewGuid(),
                    CashregisterLogId = id,
                    ProductId = l.Product.Id,
                    Amount = l.Amount,
                    ReturnedAmount = l.Returned,
                    PriceAtTimeOfSale = l.Product.Price,
                }).ToList(),
            };
        }
    }
}
