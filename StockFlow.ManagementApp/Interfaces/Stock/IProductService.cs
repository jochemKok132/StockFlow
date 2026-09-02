using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;

namespace StockFlow.ManagementApp.Interfaces.Stock
{
    public interface IProductService
    {
        Task<(List<ProductBulkViewDto>, string error)> GetPaginatedProductsAsync(ProductPaginationDto paginationDto);
        Task<(ProductDto, string error)> GetProductDetailsAsync(Guid id);
        Task<string> CreateProductAsync(CreateProductDto product);
        Task<string> UpdateProductAsync(UpdateProductDto product);
        Task<string> SoftDeleteProductAsync(Guid id);
    }
}

