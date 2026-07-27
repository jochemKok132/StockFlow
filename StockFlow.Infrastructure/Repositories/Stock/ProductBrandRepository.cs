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
    public class ProductBrandRepository :
        EfRepository<ProductBrand>,
        IEfUpdatableRepository<ProductBrand>,
        IEfCreatableRepository<ProductBrand>,
        IEfSoftDeletableRepository<ProductBrand>,
        IProductBrandRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<ProductBrand> _creatable;
        private readonly EfUpdatableRepository<ProductBrand> _updatable;
        private readonly EfSoftDeletableRepository<ProductBrand> _softDeletable;

        public ProductBrandRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<ProductBrand>(context);
            _updatable = new EfUpdatableRepository<ProductBrand>(context);
            _softDeletable = new EfSoftDeletableRepository<ProductBrand>(context);
        }

        public Task AddAsync(ProductBrand entity) => _creatable.AddAsync(entity);
        public Task UpdateAsync(ProductBrand entity) => _updatable.UpdateAsync(entity);
        public Task SoftDeleteAsync(ProductBrand entity) => _softDeletable.SoftDeleteAsync(entity);


        public async Task<IEnumerable<ProductBrand>> GetAllProductBrandsAsync(ProductBrandPaginationDto pagination)
        {
            var query = _context.ProductBrands
                .AsNoTracking()
                .AsQueryable();

            query = pagination.OrderType == OrderType.Descending
                ? pagination.OrderBy switch
                {
                    ProductBrandOrderBy.BrandName => query.OrderByDescending(p => p.BrandName),
                    ProductBrandOrderBy.ProductAmount => query.OrderByDescending(p => p.Products.Count()),
                    ProductBrandOrderBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
                    ProductBrandOrderBy.UpdatedAt => query.OrderByDescending(p => p.UpdatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt),
                }
                : pagination.OrderBy switch
                {
                    ProductBrandOrderBy.BrandName => query.OrderBy(p => p.BrandName),
                    ProductBrandOrderBy.ProductAmount => query.OrderBy(p => p.Products.Count()),
                    ProductBrandOrderBy.CreatedAt => query.OrderBy(p => p.CreatedAt),
                    ProductBrandOrderBy.UpdatedAt => query.OrderBy(p => p.UpdatedAt),
                    _ => query.OrderBy(p => p.CreatedAt),
                };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }
    }
}