using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories.EfRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockFlow.Infrastructure.Repositories.Stock
{
    public class ShelfRepository :
        EfRepository<Shelf>,
        IEfUpdatableRepository<Shelf>,
        IEfCreatableRepository<Shelf>,
        IEfSoftDeletableRepository<Shelf>,
        IShelfRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<Shelf> _creatable;
        private readonly EfUpdatableRepository<Shelf> _updatable;
        private readonly EfSoftDeletableRepository<Shelf> _softDeletable;

        public ShelfRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<Shelf>(context);
            _updatable = new EfUpdatableRepository<Shelf>(context);
            _softDeletable = new EfSoftDeletableRepository<Shelf>(context);
        }

        public Task AddAsync(Shelf entity) => _creatable.AddAsync(entity);
        public Task UpdateAsync(Shelf entity) => _updatable.UpdateAsync(entity);
        public Task SoftDeleteAsync(Shelf entity) => _softDeletable.SoftDeleteAsync(entity);


        public async Task<IEnumerable<Shelf>> GetAllShelvesAsync(ShelfPaginationDto pagination)
        {
            var query = _context.Shelves
                .AsNoTracking()
                .Include(p => p.Products)
                .Where(e =>
                e.ShelfName.ToLower().Contains((pagination.ShelfName ?? string.Empty).ToLower()) &&
                e.ShelfLocation.ToLower().Contains((pagination.Location ?? string.Empty).ToLower()));

            query = pagination.OrderType == OrderType.Descending
                ? pagination.OrderBy switch
                {
                    ShelfOrderBy.ShelfName => query.OrderByDescending(p => p.ShelfName),
                    ShelfOrderBy.ShelfLocation => query.OrderByDescending(p => p.ShelfLocation),
                    ShelfOrderBy.ProductAmount => query.OrderByDescending(p => p.Products.Count()),
                    ShelfOrderBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
                    ShelfOrderBy.UpdatedAt => query.OrderByDescending(p => p.UpdatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt),
                }
                : pagination.OrderBy switch
                {
                    ShelfOrderBy.ShelfName => query.OrderBy(p => p.ShelfName),
                    ShelfOrderBy.ShelfLocation => query.OrderBy(p => p.ShelfLocation),
                    ShelfOrderBy.ProductAmount => query.OrderBy(p => p.Products.Count()),
                    ShelfOrderBy.CreatedAt => query.OrderBy(p => p.CreatedAt),
                    ShelfOrderBy.UpdatedAt => query.OrderBy(p => p.UpdatedAt),
                    _ => query.OrderBy(p => p.CreatedAt),
                };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }
    }
}