using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.ProductBrand;

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

