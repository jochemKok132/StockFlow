using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Stock.ProductBrand;

namespace StockFlow.ManagementApp.Interfaces.Stock
{
    public interface IProductBrandService
    {
        Task<(List<ProductBrandDto>, string error)> GetPaginatedProductBrandsAsync(ProductBrandPaginationDto paginationDto);
        Task<string> CreateProductBrandAsync(CreateProductBrandDto productBrand);
        Task<string> UpdateProductBrandAsync(UpdateProductBrandDto productBrand);
        Task<string> SoftDeleteProductBrandAsync(Guid id);
    }
}

