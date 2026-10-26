using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock
{
    public interface IProductRepository : 
        IEfRepository<Product>, 
        IEfUpdatableRepository<Product>, 
        IEfCreatableRepository<Product>, 
        IEfSoftDeletableRepository<Product>
    {
        Task<Product?> GetProductByBarcodeAsync(int barcode);
        Task<IEnumerable<Product>> GetAllProductsAsync(ProductPaginationDto pagination);
        Task<IEnumerable<Product>> GetAllProductsWithTagsAsync(List<string> tags);
        Task<int> GetNewBarcode();
        Task<Dictionary<Guid, int>> GetReservedStockAsync(IEnumerable<Guid> productIds);
        Task<Dictionary<Guid, int>> GetOnOrderAsync(IEnumerable<Guid> productIds);
        Task<List<Product>> GetProductsWithPricesAsync();
        Task<List<Product>> GetMinimumStockProductsAsync(bool autoOrderOnly);
        Task<List<Product>> GetProductsOnShelfAsync(Guid shelfId, bool expirableOnly);
        Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> productIds);
        Task<List<StockBatch>> GetBatchesToCountAsync(DateTime expiryLimit);
        Task<int> CountProductsForSupplierAsync(Guid supplierId);
    }
}
