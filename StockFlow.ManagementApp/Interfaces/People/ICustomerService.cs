using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;

namespace StockFlow.ManagementApp.Interfaces.People
{
    public interface ICustomerService
    {
        Task<(List<CustomerDto>, string error)> GetPaginatedCustomersAsync(CustomerPaginationDto paginationDto);
    }
}
