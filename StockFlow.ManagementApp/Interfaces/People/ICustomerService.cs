using StockFlow.Application.DTOs.Customer;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;

namespace StockFlow.ManagementApp.Interfaces.People
{
    public interface ICustomerService
    {
        Task<(List<CustomerDto>, string error)> GetPaginatedCustomersAsync(CustomerPaginationDto paginationDto);
    }
}
