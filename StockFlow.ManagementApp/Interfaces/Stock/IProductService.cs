using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;

namespace StockFlow.ManagementApp.Interfaces.Stock
{
    public interface IProductService
    {
        Task<(List<ProductBulkViewDto>, string error)> GetPaginatedProductsAsync(ProductPaginationDto paginationDto);
        Task<(ProductDto, string error)> GetProductDetailsAsync(Guid id);
        Task<string> CreateProductAsync(CreateProductDto product);
        Task<string> UpdateProductAsync(UpdateProductDto product);
        Task<string> SoftDeleteProductAsync(Guid id);
        Task<(int? stock, string message)> WriteOffAsync(WriteOffRequest request);
    }
}

