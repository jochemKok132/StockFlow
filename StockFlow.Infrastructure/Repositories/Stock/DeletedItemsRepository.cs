using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Infrastructure.Data;

namespace StockFlow.Infrastructure.Repositories.Stock
{
    public class DeletedItemsRepository(ApplicationDBContext context) : IDeletedItemsRepository
    {
        public async Task<IEnumerable<Product>> GetDeletedProductsAsync() =>
            await context.Products.AsNoTracking().Include(p => p.Brand).Where(p => p.SoftDeleted).ToListAsync();

        public async Task<IEnumerable<ProductBrand>> GetDeletedBrandsAsync() =>
            await context.ProductBrands.AsNoTracking().Where(b => b.SoftDeleted).ToListAsync();

        public async Task<IEnumerable<Sales>> GetDeletedSalesAsync() =>
            await context.Sales.AsNoTracking().Where(s => s.SoftDeleted).ToListAsync();

        public async Task<IEnumerable<Shelf>> GetDeletedShelvesAsync() =>
            await context.Shelves.AsNoTracking().Where(s => s.SoftDeleted).ToListAsync();

        public async Task RestoreAsync<T>(T entity) where T : class, ISoftDeletable
        {
            entity.SoftDeleted = false;
            entity.SoftDeletedAt = default;
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
