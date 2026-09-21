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
    public class SalesRepository :
        EfRepository<Sales>,
        IEfUpdatableRepository<Sales>,
        IEfCreatableRepository<Sales>,
        IEfSoftDeletableRepository<Sales>,
        ISalesRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<Sales> _creatable;
        private readonly EfUpdatableRepository<Sales> _updatable;
        private readonly EfSoftDeletableRepository<Sales> _softDeletable;

        public SalesRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<Sales>(context);
            _updatable = new EfUpdatableRepository<Sales>(context);
            _softDeletable = new EfSoftDeletableRepository<Sales>(context);
        }

        public Task AddAsync(Sales entity) => _creatable.AddAsync(entity);
        public Task UpdateAsync(Sales entity) => _updatable.UpdateAsync(entity);
        public Task SoftDeleteAsync(Sales entity) => _softDeletable.SoftDeleteAsync(entity);


        public async Task<IEnumerable<Sales>> GetAllSalesAsync(SalesPaginationDto pagination)
        {
            var query = _context.Sales
                .AsNoTracking()
                .Include(p => p.Products)
                .Where(e =>
                    !e.SoftDeleted &&
                    e.SaleName.ToLower().Contains((pagination.SalesName ?? string.Empty).ToLower()));

            query = pagination.OrderType == OrderType.Descending
                ? pagination.OrderBy switch
                {
                    SalesOrderBy.SaleName => query.OrderByDescending(p => p.SaleName),
                    SalesOrderBy.PercentageOff => query.OrderByDescending(p => p.PercentageOff),
                    SalesOrderBy.ProductAmount => query.OrderByDescending(p => p.Products.Count()),
                    SalesOrderBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
                    SalesOrderBy.UpdatedAt => query.OrderByDescending(p => p.UpdatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt),
                }
                : pagination.OrderBy switch
                {
                    SalesOrderBy.SaleName => query.OrderBy(p => p.SaleName),
                    SalesOrderBy.PercentageOff => query.OrderBy(p => p.PercentageOff),
                    SalesOrderBy.ProductAmount => query.OrderBy(p => p.Products.Count()),
                    SalesOrderBy.CreatedAt => query.OrderBy(p => p.CreatedAt),
                    SalesOrderBy.UpdatedAt => query.OrderBy(p => p.UpdatedAt),
                    _ => query.OrderBy(p => p.CreatedAt),
                };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }
        public async Task<IEnumerable<Sales>> GetAllSalesForProductTags(List<string> productTags)
        {
            return await _context.Sales
                .Where(s => s.SalesTags.Any() && s.SalesTags.All(t => productTags.Contains(t)))
                .ToListAsync();
        }
        public async Task<Sales?> GetByIdWithProductsAsync(Guid id)
        {
            return await _context.Sales
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}