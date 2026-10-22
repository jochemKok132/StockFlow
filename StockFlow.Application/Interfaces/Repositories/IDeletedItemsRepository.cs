using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.Stock;

namespace StockFlow.Application.Interfaces.Repositories
{
    public interface IDeletedItemsRepository
    {
        Task<IEnumerable<Product>> GetDeletedProductsAsync();
        Task<IEnumerable<ProductBrand>> GetDeletedBrandsAsync();
        Task<IEnumerable<Sales>> GetDeletedSalesAsync();
        Task<IEnumerable<Shelf>> GetDeletedShelvesAsync();
        Task RestoreAsync<T>(T entity) where T : class, ISoftDeletable;
    }
}
