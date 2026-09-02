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
    public class ProductRepository :
        EfRepository<Product>,
        IEfUpdatableRepository<Product>,
        IEfCreatableRepository<Product>,
        IEfSoftDeletableRepository<Product>,
        IProductRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<Product> _creatable;
        private readonly EfUpdatableRepository<Product> _updatable;
        private readonly EfSoftDeletableRepository<Product> _softDeletable;

        public ProductRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<Product>(context);
            _updatable = new EfUpdatableRepository<Product>(context);
            _softDeletable = new EfSoftDeletableRepository<Product>(context);
        }

        public Task AddAsync(Product entity) => _creatable.AddAsync(entity);
        public Task UpdateAsync(Product entity) => _updatable.UpdateAsync(entity);
        public Task SoftDeleteAsync(Product entity) => _softDeletable.SoftDeleteAsync(entity);


        public async Task<IEnumerable<Product>> GetAllProductsAsync(ProductPaginationDto pagination)
        {
            pagination.ProductName = pagination.ProductName?.ToLower();

            pagination.Location ??= string.Empty;

            string? shelfLocation = null;
            string? productLocation = null;
            var hasCombo = pagination.Location.Contains('-');

            if (hasCombo)
            {
                var parts = pagination.Location.Split('-', 2);
                shelfLocation = parts[0];
                productLocation = parts.Length > 1 ? parts[1] : string.Empty;
            }

            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Shelf)
                .Where(e =>
                    e.ProductName.ToLower().Contains((pagination.ProductName ?? string.Empty).ToLower()) &&
                    e.Barcode.ToString().Contains(pagination.Barcode.ToString()) &&
                    (
                        (hasCombo &&
                            e.Shelf.ShelfLocation.ToString().StartsWith(shelfLocation ?? string.Empty) &&
                            e.Location.ToString().StartsWith(productLocation ?? string.Empty))
                        ||
                        (!hasCombo &&
                            (e.Location.ToString().StartsWith(pagination.Location) ||
                             e.Shelf.ShelfLocation.ToString().StartsWith(pagination.Location)))
                    ));

            query = pagination.OrderType == OrderType.Descending
                ? pagination.OrderBy switch
                {
                    ProductOrderBy.ProductName => query.OrderByDescending(p => p.ProductName),
                    ProductOrderBy.Stock => query.OrderByDescending(p => p.Stock),
                    ProductOrderBy.Brand => query.OrderByDescending(p => p.Brand.BrandName),
                    ProductOrderBy.Shelf => query.OrderByDescending(p => p.Shelf.ShelfName),
                    ProductOrderBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
                    ProductOrderBy.UpdatedAt => query.OrderByDescending(p => p.UpdatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt),
                }
                : pagination.OrderBy switch
                {
                    ProductOrderBy.ProductName => query.OrderBy(p => p.ProductName),
                    ProductOrderBy.Stock => query.OrderBy(p => p.Stock),
                    ProductOrderBy.Brand => query.OrderBy(p => p.Brand.BrandName),
                    ProductOrderBy.Shelf => query.OrderBy(p => p.Shelf.ShelfName),
                    ProductOrderBy.CreatedAt => query.OrderBy(p => p.CreatedAt),
                    ProductOrderBy.UpdatedAt => query.OrderBy(p => p.UpdatedAt),
                    _ => query.OrderBy(p => p.CreatedAt),
                };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithTags(List<string> tags)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(p => tags.All(t => p.SalesTags.Contains(t)))
                .ToListAsync();
        }
    }
}