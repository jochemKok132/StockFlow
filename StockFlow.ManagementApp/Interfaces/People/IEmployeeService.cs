using StockFlow.Application.DTOs.Employee;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.ProductBrand;

namespace StockFlow.ManagementApp.Interfaces.People
{
    public interface IEmployeeService
    {
        Task<(List<EmployeeDto>, string error)> GetPaginatedEmployeesAsync(EmployeePaginationDto paginationDto);
        Task<string> CreateEmployeeAsync(CreateEmployeeDto Employee);
        Task<string> UpdateEmployeeAsync(UpdateEmployeeDto Employee);
        Task<string> SoftDeleteEmployeeAsync(Guid id);
    }
}
