using StockFlow.Application.DTOs.Employee;
using StockFlow.Application.DTOs.Employee.EmployeeAuthentication;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.ManagementApp
{
    public interface IEmployeeService
    {
        Task<Token> LoginAsync(LoginRequest request);
        Task CreateEmployeeAsync(CreateEmployeeDto dto);
        Task UpdateEmployeeAsync(UpdateEmployeeDto dto);
        Task<EmployeeDto> GetEmployeeByEmployeeIdAsync(string id);
        Task<EmployeeDto> GetEmployeeByIdAsync(Guid id);
        Task<List<EmployeeDto>> GetAllEmployeesAsync(EmployeePaginationDto pagination);
    }
}
