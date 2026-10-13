using StockFlow.Shared.DTOs.Employee;
using StockFlow.Shared.DTOs.Employee.EmployeeAuthentication;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.ManagementApp
{
    public interface IEmployeeService
    {
        Task<Token> LoginAsync(LoginRequest request);
        Task CreateEmployeeAsync(CreateEmployeeDto dto, Guid userId);
        Task UpdateEmployeeAsync(UpdateEmployeeDto dto, Guid userId);
        Task<EmployeeDto> GetEmployeeByEmployeeIdAsync(string id);
        Task<EmployeeDto> GetEmployeeByIdAsync(Guid id);
        Task<List<EmployeeDto>> GetAllEmployeesAsync(EmployeePaginationDto pagination);
    }
}
