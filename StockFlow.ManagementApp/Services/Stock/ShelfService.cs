using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.Shelf;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.Stock;

namespace StockFlow.ManagementApp.Services.Stock
{
    public class ShelfService(IHttpService httpService) : IShelfService
    {

        public async Task<string> CreateShelfAsync(CreateShelfDto sale)
        {
            var response = await httpService.PostAsync<string, CreateShelfDto>($"Shelf/Create", sale);
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }

        public async Task<(List<ShelfDto>, string error)> GetPaginatedShelfAsync(ShelfPaginationDto pagination)
        {
            var url = "Shelf/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&location={pagination.Location}" +
                $"&shelfName={pagination.ShelfName}";

            var response = await httpService.GetAsync<List<ShelfDto>>(url);
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new List<ShelfDto>(), $"Error: {response.Message}");
        }

        public async Task<string> SoftDeleteShelfAsync(Guid id)
        {
            var response = await httpService.DeleteAsync<string>($"Shelf/SoftDelete/{id}");
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }

        public async Task<string> UpdateShelfAsync(UpdateShelfDto product)
        {
            var response = await httpService.PutAsync<string, UpdateShelfDto>($"Shelf/Update", product);
            if (response.Succeeded)
                return response?.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }
    }
}
