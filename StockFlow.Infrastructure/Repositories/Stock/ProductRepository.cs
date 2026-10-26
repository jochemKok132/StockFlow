using StockFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Pagination;
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

        public new async Task<Product?> GetByIdAsync(Guid id)
        {
            return await WithDetails().FirstOrDefaultAsync(e => e.Id == id);
        }

        private IQueryable<Product> WithDetails() =>
            _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Shelf)
                .Include(p => p.Supplier)
                .Include(p => p.Sales)
                .Include(p => p.Prices).ThenInclude(p => p.CreatedBy)
                .Include(p => p.Batches)
                .AsSplitQuery();
        public async Task<Product?> GetProductByBarcodeAsync(int barcode)
        {
            return await WithDetails().FirstOrDefaultAsync(e => e.Barcode == barcode);
        }
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
                .Include(p => p.Sales)
                .Where(e =>
                    !e.SoftDeleted &&
                    e.ProductName.ToLower().Contains((pagination.ProductName ?? string.Empty).ToLower()) &&
                    e.Barcode.ToString().Contains(pagination.Barcode.ToString()) &&
                    (
                        (hasCombo &&
                            e.Shelf!.ShelfLocation.ToString().StartsWith(shelfLocation ?? string.Empty) &&
                            e.Location.ToString().StartsWith(productLocation ?? string.Empty))
                        ||
                        (!hasCombo &&
                            (e.Location.ToString().StartsWith(pagination.Location) ||
                             e.Shelf!.ShelfLocation.ToString().StartsWith(pagination.Location)))
                    ));

            DateTime today = DateTime.Today;
            if (pagination.BrandId is Guid brandId)
                query = query.Where(e => e.BrandId == brandId);
            if (pagination.SupplierId is Guid supplierId)
                query = query.Where(e => e.SupplierId == supplierId);
            if (pagination.Status is ProductStatusDto status)
            {
                var domainStatus = (ProductStatus)(int)status;
                query = query.Where(e => e.Status == domainStatus);
            }
            if (pagination.SaleId is Guid saleId)
                query = query.Where(e => e.Sales.Any(s => s.Id == saleId));
            if (pagination.OnSale is bool onSale)
                query = query.Where(e => e.Sales.Any(s =>
                    !s.SoftDeleted && s.SaleType != SaleType.Moving &&
                    (s.StartDate == null || s.StartDate <= today) &&
                    (s.EndDate == null || s.EndDate >= today)) == onSale);
            if (pagination.BelowMinimum == true)
                query = query.Where(e => e.MinimumStock > 0 &&
                    e.Stock - _context.CustomerOrderLines
                        .Where(l => l.ProductId == e.Id &&
                                    (l.CustomerOrder!.Status == CustomerOrderStatus.Waiting || l.CustomerOrder.Status == CustomerOrderStatus.ReadyForPickup))
                        .Sum(l => l.ReservedAmount) < e.MinimumStock);

            query = pagination.OrderType == OrderType.Descending
                ? pagination.OrderBy switch
                {
                    ProductOrderBy.ProductName => query.OrderByDescending(p => p.ProductName),
                    ProductOrderBy.Stock => query.OrderByDescending(p => p.Stock),
                    ProductOrderBy.Brand => query.OrderByDescending(p => p.Brand!.BrandName),
                    ProductOrderBy.Shelf => query.OrderByDescending(p => p.Shelf!.ShelfName),
                    ProductOrderBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
                    ProductOrderBy.UpdatedAt => query.OrderByDescending(p => p.UpdatedAt),
                    ProductOrderBy.Price => query.OrderByDescending(p => (double)p.Price),
                    ProductOrderBy.Sales => query.OrderByDescending(p => p.Sales.Count(s => !s.SoftDeleted)),
                    _ => query.OrderByDescending(p => p.CreatedAt),
                }
                : pagination.OrderBy switch
                {
                    ProductOrderBy.ProductName => query.OrderBy(p => p.ProductName),
                    ProductOrderBy.Stock => query.OrderBy(p => p.Stock),
                    ProductOrderBy.Brand => query.OrderBy(p => p.Brand!.BrandName),
                    ProductOrderBy.Shelf => query.OrderBy(p => p.Shelf!.ShelfName),
                    ProductOrderBy.CreatedAt => query.OrderBy(p => p.CreatedAt),
                    ProductOrderBy.UpdatedAt => query.OrderBy(p => p.UpdatedAt),
                    ProductOrderBy.Price => query.OrderBy(p => (double)p.Price),
                    ProductOrderBy.Sales => query.OrderBy(p => p.Sales.Count(s => !s.SoftDeleted)),
                    _ => query.OrderBy(p => p.CreatedAt),
                };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithTagsAsync(List<string> tags)
        {
            return await _context.Products
                .Where(p => tags.All(t => p.SalesTags.Contains(t)))
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, int>> GetReservedStockAsync(IEnumerable<Guid> productIds)
        {
            var ids = productIds.Distinct().ToList();
            var rows = await _context.CustomerOrderLines
                .AsNoTracking()
                .Where(l => ids.Contains(l.ProductId) &&
                            (l.CustomerOrder!.Status == CustomerOrderStatus.Waiting || l.CustomerOrder.Status == CustomerOrderStatus.ReadyForPickup))
                .GroupBy(l => l.ProductId)
                .Select(g => new { g.Key, Amount = g.Sum(l => l.ReservedAmount) })
                .ToListAsync();
            return rows.ToDictionary(r => r.Key, r => r.Amount);
        }

        public async Task<Dictionary<Guid, int>> GetOnOrderAsync(IEnumerable<Guid> productIds)
        {
            var ids = productIds.Distinct().ToList();
            var rows = await _context.PurchaseOrderLines
                .AsNoTracking()
                .Where(l => ids.Contains(l.ProductId) &&
                            (l.PurchaseOrder!.Status == PurchaseOrderStatus.Open || l.PurchaseOrder.Status == PurchaseOrderStatus.Sent))
                .GroupBy(l => l.ProductId)
                .Select(g => new { g.Key, Amount = g.Sum(l => l.Amount) })
                .ToListAsync();
            return rows.ToDictionary(r => r.Key, r => r.Amount);
        }

        public Task<List<Product>> GetProductsWithPricesAsync() =>
            _context.Products
                .Include(p => p.Prices)
                .Where(p => !p.SoftDeleted && p.Prices.Any())
                .ToListAsync();

        public Task<List<Product>> GetMinimumStockProductsAsync(bool autoOrderOnly) =>
            _context.Products
                .Include(p => p.Supplier)
                .Where(p => !p.SoftDeleted && p.Status == ProductStatus.Active && p.MinimumStock > 0 && (!autoOrderOnly || p.AutoOrder))
                .ToListAsync();

        public Task<List<Product>> GetProductsOnShelfAsync(Guid shelfId, bool expirableOnly) =>
            _context.Products
                .Where(p => !p.SoftDeleted && p.ShelfId == shelfId && (!expirableOnly || p.CanExpire))
                .OrderBy(p => p.Location)
                .ThenBy(p => p.ProductName)
                .ToListAsync();

        public Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds)
        {
            var ids = productIds.Distinct().ToList();
            return _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
        }

        public Task<List<StockBatch>> GetBatchesToCountAsync(DateTime expiryLimit) =>
            _context.StockBatches
                .Include(b => b.Product)
                .Where(b => b.Amount > 0 && !b.CountForced && b.ExpiryDate <= expiryLimit &&
                            !b.Product!.SoftDeleted && b.Product.CanExpire)
                .ToListAsync();

        public Task<int> CountProductsForSupplierAsync(Guid supplierId) =>
            _context.Products.CountAsync(p => !p.SoftDeleted && p.SupplierId == supplierId);

        public async Task<int> GetNewBarcode()
        {
            var highest = await _context.Products.MaxAsync(p => (int?)p.Barcode) ?? 500000000;
            return highest + 1;
        }
    }
}