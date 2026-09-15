using StockFlow.Application.DTOs.Employee;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Sales;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.People;

namespace StockFlow.ManagementApp.Services.People
{
    public class EmployeeService(IHttpService httpService) : IEmployeeService
    {
        public async Task<(List<EmployeeDto>, string error)> GetPaginatedEmployeesAsync(EmployeePaginationDto pagination)
        {
            var url = "Employee/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&employeeName={pagination.EmployeeName}" +
                $"&employeeId={pagination.EmployeeId}" +
                $"&role={pagination.Role}";

            var response = await httpService.GetAsync<List<EmployeeDto>>(url);
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new List<EmployeeDto>(), $"Error: {response.Message}");
        }
    }
}
