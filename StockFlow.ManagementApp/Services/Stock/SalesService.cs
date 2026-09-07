using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.Sales;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.Stock;

namespace StockFlow.ManagementApp.Services.Stock
{
    public class SalesService(IHttpService httpService) : ISalesService
    {

        public async Task<string> CreateSalesAsync(CreateSalesDto sale)
        {
            var response = await httpService.PostAsync<string, CreateSalesDto>($"Sales/Create", sale);
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }

        public async Task<(List<SalesDto>, string error)> GetPaginatedSalesAsync(SalesPaginationDto pagination)
        {
            var url = "Sales/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&saleName={pagination.SalesName}";

            var response = await httpService.GetAsync<List<SalesDto>>(url);
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new List<SalesDto>(), $"Error: {response.Message}");
        }

        public async Task<string> SoftDeleteSalesAsync(Guid id)
        {
            var response = await httpService.GetAsync<string>($"Sales/SoftDelete/{id}");
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }

        public async Task<string> UpdateSalesAsync(UpdateSalesDto product)
        {
            var response = await httpService.PutAsync<string, UpdateSalesDto>($"Sales/Update", product);
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }
    }
}
