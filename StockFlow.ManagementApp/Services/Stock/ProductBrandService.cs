using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.ProductBrand;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.Stock;

namespace StockFlow.ManagementApp.Services.Stock
{
    public class ProductBrandService(IHttpService httpService) : IProductBrandService
    {

        public async Task<string> CreateProductBrandAsync(CreateProductBrandDto sale)
        {
            var response = await httpService.PostAsync<string, CreateProductBrandDto>($"ProductBrand/Create", sale);
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }

        public async Task<(List<ProductBrandDto>, string error)> GetPaginatedProductBrandsAsync(ProductBrandPaginationDto pagination)
        {
            var url = "ProductBrand/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&brandName={pagination.BrandName}";

            var response = await httpService.GetAsync<List<ProductBrandDto>>(url);
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new List<ProductBrandDto>(), $"Error: {response.Message}");
        }

        public async Task<string> SoftDeleteProductBrandAsync(Guid id)
        {
            var response = await httpService.DeleteAsync<string>($"ProductBrand/SoftDelete/{id}");
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }

        public async Task<string> UpdateProductBrandAsync(UpdateProductBrandDto product)
        {
            var response = await httpService.PutAsync<string, UpdateProductBrandDto>($"ProductBrand/Update", product);
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }
    }
}
