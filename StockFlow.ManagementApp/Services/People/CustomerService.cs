using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Sales;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.People;

namespace StockFlow.ManagementApp.Services.People
{
    public class CustomerService(IHttpService httpService) : ICustomerService
    {
        public async Task<(List<CustomerDto>, string error)> GetPaginatedCustomersAsync(CustomerPaginationDto pagination)
        {
            var url = "Customer/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&customerId={pagination.CustomerId}" +
                $"&email={pagination.Email}" +
                $"&houseNumber={pagination.HouseNumber}" +
                $"&postalCode={pagination.PostalCode}" +
                $"&customerName={pagination.CustomerName}";

            var response = await httpService.GetAsync<List<CustomerDto>>(url);
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new List<CustomerDto>(), $"Error: {response.Message}");
        }

        public async Task<(List<CustomerDto>, string error)> GetMovingSaleRequestsAsync()
        {
            var response = await httpService.GetAsync<List<CustomerDto>>("Customer/MovingSaleRequests");
            return response.Succeeded ? (response.Value ?? [], "") : ([], response.Message ?? "The requests could not be loaded.");
        }

        public Task<string> ApproveMovingSaleAsync(Guid customerId) => ReviewAsync(customerId, "Approve");

        public Task<string> RejectMovingSaleAsync(Guid customerId) => ReviewAsync(customerId, "Reject");

        private async Task<string> ReviewAsync(Guid customerId, string action)
        {
            var response = await httpService.PostAsync<CustomerDto, object>($"Customer/MovingSale/{customerId}/{action}", new { });
            return response.Succeeded ? "" : response.Message ?? "The request could not be saved.";
        }
    }
}
