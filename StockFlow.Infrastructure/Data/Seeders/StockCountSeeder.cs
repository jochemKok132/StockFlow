using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class StockCountSeeder
    {
        public static void UseStockCountSeeder(this DbContext context)
        {
            if (context.Set<StockCount>().Any() || !context.Set<Product>().Any())
                return;

            var screws = context.Set<Product>().Where(p => p.ShelfId == SeedIds.ScrewsShelf && !p.SoftDeleted).OrderBy(p => p.Location).ToList();
            var locks = context.Set<Product>().Where(p => p.ShelfId == SeedIds.LocksShelf && !p.SoftDeleted).OrderBy(p => p.Location).ToList();

            var completed = new StockCount
            {
                Id = Guid.NewGuid(),
                CountNumber = 1001,
                Type = StockCountType.Manual,
                Status = StockCountStatus.Completed,
                ShelfId = SeedIds.ScrewsShelf,
                Note = "Monthly count of the screws.",
                CreatedById = SeedIds.ShiftLeaderOne,
                CreatedAt = SeedIds.DaysAgo(11, 9),
                UpdatedAt = SeedIds.DaysAgo(10, 17),
                CompletedAt = SeedIds.DaysAgo(10, 17),
                CompletedById = SeedIds.FloorWorkerTwo,
                Lines = screws.Select((p, index) => new StockCountLine
                {
                    ProductId = p.Id,
                    ExpectedStock = index == 2 ? p.Stock + 2 : index == 5 ? p.Stock - 1 : p.Stock,
                    CountedStock = p.Stock,
                }).ToList(),
            };

            var open = new StockCount
            {
                Id = Guid.NewGuid(),
                CountNumber = 1002,
                Type = StockCountType.Manual,
                Status = StockCountStatus.Open,
                ShelfId = SeedIds.LocksShelf,
                DueDate = DateTime.Today.AddDays(1),
                Note = "Count the locks before the sale starts.",
                CreatedById = SeedIds.ShiftLeaderOne,
                CreatedAt = SeedIds.DaysAgo(0, 8),
                UpdatedAt = SeedIds.DaysAgo(0, 8),
                Lines = locks.Select(p => new StockCountLine { ProductId = p.Id, ExpectedStock = p.Stock }).ToList(),
            };

            context.Set<StockCount>().AddRange(completed, open);
            context.SaveChanges();
        }
    }
}
