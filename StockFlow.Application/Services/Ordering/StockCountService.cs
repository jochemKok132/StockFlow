using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.Ordering;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.Ordering;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Mappings.Ordering;
using StockFlow.Application.Validations.Stock;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Ordering;

namespace StockFlow.Application.Services.Ordering
{
    public class StockCountService(
        IStockCountRepository stockCountRepository,
        IProductRepository productRepository,
        IShelfRepository shelfRepository,
        IStockLogsService stockLogsService,
        IStockAllocator stockAllocator) : IStockCountService
    {
        public const string CountEveryProduct = "Count every product before completing the stock count.";
        public const string ExpiryCountCantBeCancelled = "A forced expiry count can't be cancelled. It has to be done.";

        public async Task<List<StockCountDto>> GetCountsAsync(StockCountStatusDto? status)
        {
            if (status is { } value && !Enum.IsDefined(value))
                throw new ArgumentException("Choose a valid status.");
            var counts = await stockCountRepository.GetListAsync(status?.ToDomain());
            return counts.Select(c => c.ToStockCountDto()).ToList();
        }

        public async Task<StockCountDto> GetByNumberAsync(int countNumber) =>
            (await GetCountAsync(countNumber)).ToStockCountDto();

        public async Task<StockCountDto> CreateAsync(CreateStockCountRequest request, Guid employeeId)
        {
            request.Validate();
            var shelf = await shelfRepository.GetByIdAsync(request.ShelfId);
            if (shelf is null || shelf.SoftDeleted)
                throw new KeyNotFoundException("Shelf not found.");

            var open = await stockCountRepository.GetOpenCountForShelfAsync(shelf.Id, StockCountType.Manual);
            if (open is not null)
                throw new ArgumentException($"Shelf {shelf.ShelfName} already has an open stock count ({open.CountNumber}).");

            var products = await productRepository.GetProductsOnShelfAsync(shelf.Id, request.ExpirableOnly);
            if (products.Count == 0)
                throw new ArgumentException(request.ExpirableOnly
                    ? $"Shelf {shelf.ShelfName} has no products that can expire."
                    : $"Shelf {shelf.ShelfName} has no products.");

            var count = new StockCount
            {
                Id = Guid.NewGuid(),
                CountNumber = await stockCountRepository.GetNextCountNumberAsync(),
                Type = StockCountType.Manual,
                Status = StockCountStatus.Open,
                ShelfId = shelf.Id,
                Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
                CreatedById = employeeId,
                Lines = products.Select(p => new StockCountLine { ProductId = p.Id, ExpectedStock = p.Stock }).ToList(),
            };
            await stockCountRepository.AddAsync(count);
            return await GetByNumberAsync(count.CountNumber);
        }

        public async Task<StockCountDto> CompleteAsync(int countNumber, CompleteStockCountRequest request, Guid employeeId)
        {
            request.Validate();
            DateTime today = DateTime.Today;
            var count = await GetCountAsync(countNumber);
            if (count.Status != StockCountStatus.Open)
                throw new ArgumentException($"Stock count {countNumber} is already {count.Status.ToString().ToLowerInvariant()}.");

            var entries = request.Lines.ToDictionary(l => l.LineId);
            if (count.Lines.Any(l => !entries.ContainsKey(l.Id)) || entries.Keys.Any(id => count.Lines.All(l => l.Id != id)))
                throw new ArgumentException(CountEveryProduct);

            var work = new List<(StockCountLine Line, Product Product, StockCountEntry Entry)>();
            foreach (var line in count.Lines)
            {
                var product = await productRepository.GetByIdAsync(line.ProductId)
                    ?? throw new KeyNotFoundException("Product not found.");
                var entry = entries[line.Id];
                if (product.CanExpire && entry.CountedStock > 0)
                {
                    if (entry.NextExpiryDate is null)
                        throw new ArgumentException($"Enter the first expiry date of {product.ProductName}.");
                    if (entry.NextExpiryDate.Value.Date <= today)
                        throw new ArgumentException($"The first expiry date of {product.ProductName} must be in the future. Write off what expires today as expired.");
                }
                work.Add((line, product, entry));
            }

            var increased = new List<Guid>();
            foreach (var (line, product, entry) in work)
            {
                int before = product.Stock;
                var logs = new List<CreateStockLogsDto>();

                if (entry.ExpiredAmount > 0)
                {
                    var writeOff = product.ChangeStock(-entry.ExpiredAmount, employeeId, LogTypeDto.WrittenOff);
                    writeOff.Messages.Add($"Reason: {WriteOffReasonDto.Expired}");
                    writeOff.Messages.Add($"Count: {count.CountNumber}");
                    product.Stock -= entry.ExpiredAmount;
                    logs.Add(writeOff);
                }

                if (product.Stock != entry.CountedStock)
                {
                    var counted = product.ChangeStock(entry.CountedStock - product.Stock, employeeId, LogTypeDto.Counted);
                    counted.Messages.Add($"Count: {count.CountNumber}");
                    product.Stock = entry.CountedStock;
                    logs.Add(counted);
                }

                if (product.CanExpire)
                {
                    product.Batches.Clear();
                    if (entry.CountedStock > 0 && entry.NextExpiryDate is DateTime expiry)
                        product.Batches.Add(new StockBatch
                        {
                            ProductId = product.Id,
                            Amount = entry.CountedStock,
                            ExpiryDate = expiry.Date,
                            CreatedAt = DateTime.UtcNow,
                            CountForced = ExpiryRules.NeedsCount(expiry, today),
                        });
                }

                line.ExpectedStock = before;
                line.CountedStock = entry.CountedStock;
                line.ExpiredAmount = entry.ExpiredAmount;
                line.NextExpiryDate = product.CanExpire ? entry.NextExpiryDate?.Date : null;
                if (product.Stock > before) increased.Add(product.Id);

                await productRepository.UpdateAsync(product);
                foreach (var log in logs)
                    await stockLogsService.CreateStockLogsAsync(log);
            }

            count.Status = StockCountStatus.Completed;
            count.CompletedAt = DateTime.UtcNow;
            count.CompletedById = employeeId;
            await stockCountRepository.UpdateAsync(count);

            await stockAllocator.AllocateAsync(increased);
            return await GetByNumberAsync(countNumber);
        }

        public async Task<StockCountDto> CancelAsync(int countNumber)
        {
            var count = await GetCountAsync(countNumber);
            if (count.Status != StockCountStatus.Open)
                throw new ArgumentException($"Stock count {countNumber} is already {count.Status.ToString().ToLowerInvariant()}.");
            if (count.Type == StockCountType.Expiry)
                throw new ArgumentException(ExpiryCountCantBeCancelled);

            count.Status = StockCountStatus.Cancelled;
            await stockCountRepository.UpdateAsync(count);
            return await GetByNumberAsync(countNumber);
        }

        public async Task<int> CreateExpiryCountsAsync(DateTime today)
        {
            var batches = await productRepository.GetBatchesToCountAsync(today.Date.AddDays(ExpiryRules.CountDaysBeforeExpiry));
            int created = 0;

            foreach (var shelfGroup in batches.Where(b => b.Product is not null).GroupBy(b => b.Product!.ShelfId))
            {
                DateTime due = shelfGroup.Min(b => ExpiryRules.CountDueDate(b.ExpiryDate, today));
                DateTime firstExpiry = shelfGroup.Min(b => b.ExpiryDate.Date);
                var open = await stockCountRepository.GetOpenCountForShelfAsync(shelfGroup.Key, StockCountType.Expiry);

                if (open is not null)
                {
                    foreach (var productId in shelfGroup.Select(b => b.ProductId).Distinct())
                    {
                        if (open.Lines.Any(l => l.ProductId == productId)) continue;
                        var product = shelfGroup.First(b => b.ProductId == productId).Product!;
                        open.Lines.Add(new StockCountLine { StockCountId = open.Id, ProductId = productId, ExpectedStock = product.Stock });
                    }
                    if (open.DueDate is null || due < open.DueDate)
                        open.DueDate = due;
                    await stockCountRepository.UpdateAsync(open);
                }
                else
                {
                    var products = await productRepository.GetProductsOnShelfAsync(shelfGroup.Key, expirableOnly: true);
                    if (products.Count == 0) continue;

                    await stockCountRepository.AddAsync(new StockCount
                    {
                        Id = Guid.NewGuid(),
                        CountNumber = await stockCountRepository.GetNextCountNumberAsync(),
                        Type = StockCountType.Expiry,
                        Status = StockCountStatus.Open,
                        ShelfId = shelfGroup.Key,
                        DueDate = due,
                        Note = $"Products on this shelf expire from {firstExpiry:dd-MM-yyyy}. Count them and take out what has expired.",
                        Lines = products.Select(p => new StockCountLine { ProductId = p.Id, ExpectedStock = p.Stock }).ToList(),
                    });
                    created++;
                }

                foreach (var batch in shelfGroup)
                    batch.CountForced = true;
                await productRepository.SaveChangesAsync();
            }

            return created;
        }

        private async Task<StockCount> GetCountAsync(int countNumber) =>
            await stockCountRepository.GetByNumberAsync(countNumber)
            ?? throw new KeyNotFoundException($"Stock count {countNumber} not found.");
    }
}
