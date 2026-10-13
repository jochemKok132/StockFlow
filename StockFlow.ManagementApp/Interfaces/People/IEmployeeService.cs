using StockFlow.Shared.DTOs.Employee;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Stock.ProductBrand;

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
