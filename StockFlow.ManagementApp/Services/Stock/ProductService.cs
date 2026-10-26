using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.AuditLogs.Customer;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
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
                $"&location={pagination.Location}" +
                (pagination.BrandId is Guid brandId ? $"&brandId={brandId}" : "") +
                (pagination.SupplierId is Guid supplierId ? $"&supplierId={supplierId}" : "") +
                (pagination.SaleId is Guid saleId ? $"&saleId={saleId}" : "") +
                (pagination.OnSale is bool onSale ? $"&onSale={onSale}" : "") +
                (pagination.Status is ProductStatusDto status ? $"&status={status}" : "") +
                (pagination.BelowMinimum == true ? "&belowMinimum=true" : "");

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
            var response = await httpService.DeleteAsync<string>($"Product/SoftDelete/{id}");
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }

        public async Task<(int? stock, string message)> WriteOffAsync(WriteOffRequest request)
        {
            var response = await httpService.PostAsync<int, WriteOffRequest>("Product/WriteOff", request);
            return response.Succeeded
                ? (response.Value, response.Message ?? "Written off.")
                : (null, response.Message ?? "The write-off could not be saved.");
        }

        public async Task<ApiResult<ProductDto>> SchedulePriceAsync(SchedulePriceRequest request) =>
            ApiResult<ProductDto>.From(await httpService.PostAsync<ProductDto, SchedulePriceRequest>("Product/Price", request), "The price could not be saved.");

        public async Task<ApiResult<ProductDto>> CancelPriceAsync(Guid productId, Guid priceId) =>
            ApiResult<ProductDto>.From(await httpService.DeleteAsync<ProductDto>($"Product/{productId}/Price/{priceId}"), "The planned price could not be cancelled.");

        public async Task<ApiResult<ProductDto>> RecallAsync(RecallRequest request) =>
            ApiResult<ProductDto>.From(await httpService.PostAsync<ProductDto, RecallRequest>("Product/Recall", request), "The product could not be recalled.");

        public async Task<ApiResult<ProductDto>> UndoRecallAsync(Guid productId) =>
            ApiResult<ProductDto>.From(await httpService.DeleteAsync<ProductDto>($"Product/{productId}/Recall"), "The recall could not be lifted.");

        public async Task<ApiResult<ProductDto>> BlockOrdersAsync(OrderBlockRequest request) =>
            ApiResult<ProductDto>.From(await httpService.PostAsync<ProductDto, OrderBlockRequest>("Product/OrderBlock", request), "The order block could not be saved.");

        public async Task<ApiResult<ProductDto>> UnblockOrdersAsync(Guid productId) =>
            ApiResult<ProductDto>.From(await httpService.DeleteAsync<ProductDto>($"Product/{productId}/OrderBlock"), "The order block could not be removed.");

        public async Task<ApiResult<ProductSalesDto>> GetSalesAsync(Guid productId, DateTime from, DateTime to) =>
            ApiResult<ProductSalesDto>.From(await httpService.GetAsync<ProductSalesDto>($"Statistics/Product/{productId}?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}"), "The sales could not be loaded.");

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
