using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories.EfRepositories;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.Infrastructure.Repositories.Stock
{
    public class SupplierRepository :
        EfRepository<Supplier>,
        IEfUpdatableRepository<Supplier>,
        IEfCreatableRepository<Supplier>,
        IEfSoftDeletableRepository<Supplier>,
        ISupplierRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<Supplier> _creatable;
        private readonly EfUpdatableRepository<Supplier> _updatable;
        private readonly EfSoftDeletableRepository<Supplier> _softDeletable;

        public SupplierRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<Supplier>(context);
            _updatable = new EfUpdatableRepository<Supplier>(context);
            _softDeletable = new EfSoftDeletableRepository<Supplier>(context);
        }

        public Task AddAsync(Supplier entity) => _creatable.AddAsync(entity);
        public Task UpdateAsync(Supplier entity) => _updatable.UpdateAsync(entity);
        public Task SoftDeleteAsync(Supplier entity) => _softDeletable.SoftDeleteAsync(entity);

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync(SupplierPaginationDto pagination)
        {
            string name = (pagination.SupplierName ?? string.Empty).ToLower();
            var query = _context.Suppliers
                .AsNoTracking()
                .Where(s => !s.SoftDeleted && s.SupplierName.ToLower().Contains(name));

            bool descending = pagination.OrderType == OrderType.Descending;
            query = pagination.OrderBy switch
            {
                SupplierOrderBy.OrderDay => descending ? query.OrderByDescending(s => s.OrderDay) : query.OrderBy(s => s.OrderDay),
                SupplierOrderBy.CreatedAt => descending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
                _ => descending ? query.OrderByDescending(s => s.SupplierName) : query.OrderBy(s => s.SupplierName),
            };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, int>> GetProductCountsAsync(IEnumerable<Guid> supplierIds)
        {
            var ids = supplierIds.Distinct().ToList();
            var rows = await _context.Products
                .AsNoTracking()
                .Where(p => !p.SoftDeleted && ids.Contains(p.SupplierId))
                .GroupBy(p => p.SupplierId)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToListAsync();
            return rows.ToDictionary(r => r.Key, r => r.Count);
        }
    }
}
