using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.Stock;

namespace StockFlow.ManagementApp.Services.Stock
{
    public class ProductService(IHttpService httpService) : IProductService
    {
        public async Task<string> CreateProductAsync(CreateProductDto product)
        {
            var response = await httpService.PostAsync<string, CreateProductDto>($"Product/Create", product);
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }

        public async Task<(List<ProductBulkViewDto>, string error)> GetPaginatedProductsAsync(ProductPaginationDto pagination)
        {
            var url = "Product/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&productName={pagination.ProductName}" +
                $"&barcode={pagination.Barcode}" +
                $"&location={pagination.Location}";

            var response = await httpService.GetAsync<List<ProductBulkViewDto>>(url);
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new List<ProductBulkViewDto>(), $"Error: {response.Message}");
        }

        public async Task<(ProductDto, string error)> GetProductDetailsAsync(Guid id)
        {
            var response = await httpService.GetAsync<ProductDto>($"Product/GetById/{id}");
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new ProductDto(), $"Error: {response.Message}");
        }

        public async Task<string> SoftDeleteProductAsync(Guid id)
        {
            var response = await httpService.GetAsync<string>($"Product/SoftDelete/{id}");
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }

        public async Task<string> UpdateProductAsync(UpdateProductDto product)
        {
            var response = await httpService.PutAsync<string, UpdateProductDto>($"Product/Update", product);
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }
    }
}
