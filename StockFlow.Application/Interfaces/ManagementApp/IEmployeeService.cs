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
        Task CreateEmployeeAsync(CreateEmployeeDto dto, Guid userId, EmployeeRoleDto actorRole = EmployeeRoleDto.None);
        Task UpdateEmployeeAsync(UpdateEmployeeDto dto, Guid userId, EmployeeRoleDto actorRole = EmployeeRoleDto.None);
        Task<EmployeeDto> GetEmployeeByEmployeeIdAsync(string id, EmployeeRoleDto viewerRole = EmployeeRoleDto.None);
        Task<EmployeeDto> GetEmployeeByIdAsync(Guid id, EmployeeRoleDto viewerRole = EmployeeRoleDto.None);
        Task<List<EmployeeDto>> GetAllEmployeesAsync(EmployeePaginationDto pagination, EmployeeRoleDto viewerRole = EmployeeRoleDto.None);
    }
}
