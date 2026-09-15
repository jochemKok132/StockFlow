using StockFlow.Application.DTOs.Employee;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;

namespace StockFlow.ManagementApp.Interfaces.People
{
    public interface IEmployeeService
    {
        Task<(List<EmployeeDto>, string error)> GetPaginatedEmployeesAsync(EmployeePaginationDto paginationDto);
    }
}
