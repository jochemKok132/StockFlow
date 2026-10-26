using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Application.Interfaces.Repositories.Ordering;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Enums;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories.EfRepositories;

namespace StockFlow.Infrastructure.Repositories.Ordering
{
    public class StockCountRepository :
        EfRepository<StockCount>,
        IEfCreatableRepository<StockCount>,
        IEfUpdatableRepository<StockCount>,
        IStockCountRepository
    {
        public const int FirstCountNumber = 1001;
        private const int MaxCounts = 300;
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<StockCount> _creatable;
        private readonly EfUpdatableRepository<StockCount> _updatable;

        public StockCountRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<StockCount>(context);
            _updatable = new EfUpdatableRepository<StockCount>(context);
        }

        public Task AddAsync(StockCount entity) => _creatable.AddAsync(entity);
        public Task UpdateAsync(StockCount entity) => _updatable.UpdateAsync(entity);

        public async Task<int> GetNextCountNumberAsync()
        {
            int? highest = await _context.StockCounts.MaxAsync(c => (int?)c.CountNumber);
            return highest is int number ? number + 1 : FirstCountNumber;
        }

        public Task<StockCount?> GetByNumberAsync(int countNumber) =>
            WithDetails().FirstOrDefaultAsync(c => c.CountNumber == countNumber);

        public Task<List<StockCount>> GetListAsync(StockCountStatus? status) =>
            WithDetails()
                .AsNoTracking()
                .Where(c => status == null || c.Status == status)
                .OrderBy(c => c.Status != StockCountStatus.Open)
                .ThenBy(c => c.DueDate == null)
                .ThenBy(c => c.DueDate)
                .ThenByDescending(c => c.CountNumber)
                .Take(MaxCounts)
                .ToListAsync();

        public Task<StockCount?> GetOpenCountForShelfAsync(Guid shelfId, StockCountType type) =>
            _context.StockCounts
                .Include(c => c.Lines)
                .FirstOrDefaultAsync(c => c.ShelfId == shelfId && c.Type == type && c.Status == StockCountStatus.Open);

        private IQueryable<StockCount> WithDetails() =>
            _context.StockCounts
                .Include(c => c.Shelf)
                .Include(c => c.CreatedBy)
                .Include(c => c.CompletedBy)
                .Include(c => c.Lines).ThenInclude(l => l.Product).ThenInclude(p => p!.Batches)
                .AsSplitQuery();
    }
}
