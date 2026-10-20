using StockFlow.Shared.DTOs.Employee;
using StockFlow.Shared.DTOs.Employee.EmployeeAuthentication;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Shelf;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Interfaces.Helpers;
using StockFlow.Application.Interfaces.ManagementApp;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Application.Interfaces.Repositories.ManagementApp;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Mappings.People;
using StockFlow.Application.Services.ManagementApp.AuditLogs;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Enums;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using StockFlow.Application.Validations;
using StockFlow.Application.Validations.People;

namespace StockFlow.Application.Services.ManagementApp
{
    public class EmployeeService(IEmployeeRepository employeeRepository, IAuthenticationService authenticationService, IPassword password, IEmployeeLogsService employeeLogsService) : IEmployeeService
    {
        public const string OnlyAdminAssignsTester = "Only an admin can give out the Tester role.";

        public async Task CreateEmployeeAsync(CreateEmployeeDto dto, Guid userId, bool byAdmin = false)
        {
            dto.Validate();

            if (dto.Role == EmployeeRoleDto.Tester && !byAdmin)
                throw new ArgumentException(OnlyAdminAssignsTester);

            Employee newEmployee = dto.ToEmployeeEntity();
            newEmployee.HashedPassword = password.Hash(dto.Password);

            newEmployee.EmployeeId = await employeeRepository.GetNewEmployeeIdAsync();

            await employeeRepository.AddAsync(newEmployee);
            await employeeLogsService.CreateEmployeeLogsAsync(dto.ToCreateEmployeeLogsDto(newEmployee.EmployeeId, userId));
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync(EmployeePaginationDto pagination, bool includeTesters = false)
        {
            pagination.Validate();

            var employees = await employeeRepository.GetAllEmployeesAsync(pagination, includeTesters);
            return employees.Select(e => e.ToEmployeeDto()).ToList();
        }

        public async Task<EmployeeDto> GetEmployeeByEmployeeIdAsync(string id, bool includeTesters = false)
        {
            var employee = await employeeRepository.GetEmployeeByEmployeeIdAsync(id);
            if (employee == null || IsHiddenTester(employee, includeTesters)) throw new KeyNotFoundException($"Employee with employee id {id} not found.");
            return employee.ToEmployeeDto();
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(Guid id, bool includeTesters = false)
        {
            var employee = await employeeRepository.GetByIdAsync(id);
            if (employee == null || IsHiddenTester(employee, includeTesters)) throw new KeyNotFoundException("Employee not found.");
            return employee.ToEmployeeDto();
        }

        private static bool IsHiddenTester(Employee employee, bool includeTesters) =>
            employee.Role == EmployeeRole.Tester && !includeTesters;

        public async Task<Token> LoginAsync(LoginRequest request)
        {
            request.Validate();

            Employee? employee = await employeeRepository.GetEmployeeByEmployeeIdAsync(request.EmployeeId);

            if (employee == null)
                throw new KeyNotFoundException("The employee with this id cannot be found.");
            
            if (string.IsNullOrEmpty(employee.HashedPassword) || !password.Validate(employee.HashedPassword, request.Password))
                throw new AuthenticationException("The password is incorrect.");
            await employeeLogsService.CreateEmployeeLogsAsync(employee.ToLoginEmployeeLogsDto());

            return authenticationService.GenerateToken(employee);
        }

        public async Task UpdateEmployeeAsync(UpdateEmployeeDto dto, Guid userId, bool byAdmin = false)
        {
            dto.Validate();

            var employee = await employeeRepository.GetByIdAsync(dto.Id);
            if (employee == null || IsHiddenTester(employee, byAdmin))
                throw new KeyNotFoundException("Employee not found.");

            if (dto.Role == EmployeeRoleDto.Tester && !byAdmin)
                throw new ArgumentException(OnlyAdminAssignsTester);
            var log = dto.ToUpdateEmployeeLogsDto(employee, userId);

            employee.ToEmployeeEntity(dto);

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                employee.HashedPassword = password.Hash(dto.NewPassword);
                log.Messages.Add("Password: Changed.");
            }
            await employeeRepository.UpdateAsync(employee);
            await employeeLogsService.CreateEmployeeLogsAsync(log);

        }
    }
}
