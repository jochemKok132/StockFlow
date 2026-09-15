using StockFlow.Application.DTOs.Customer;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Sales;
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
    }
}
