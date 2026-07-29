using StockFlow.Application.DTOs.Employee;
using StockFlow.Application.DTOs.Employee.EmployeeAuthentication;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Interfaces.Helpers;
using StockFlow.Application.Interfaces.ManagementApp;
using StockFlow.Application.Interfaces.Repositories.ManagementApp;
using StockFlow.Application.Mappings.People;
using StockFlow.Domain.Entities.People;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace StockFlow.Application.Services.ManagementApp
{
    public class EmployeeService(IEmployeeRepository employeeRepository, IAuthenticationService authenticationService, IPassword password) : IEmployeeService
    {
        public async Task CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            Employee newEmployee = dto.ToEmployeeEntity();

            newEmployee.EmployeeId = await employeeRepository.GetNewEmployeeIdAsync();

            await employeeRepository.AddAsync(newEmployee);
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await employeeRepository.GetAllAsync();
            return employees.Select(e => e.ToEmployeeDto()).ToList();
        }

        public async Task<EmployeeDto> GetEmployeeByEmployeeIdAsync(string id)
        {
            var employee = await employeeRepository.GetEmployeeByEmployeeIdAsync(id);
            return employee.ToEmployeeDto();
        }

        public async Task<Token> LoginAsync(LoginRequest request)
        {
            Employee? employee = await employeeRepository.GetEmployeeByEmployeeIdAsync(request.EmployeeId);

            if (employee == null)
                throw new KeyNotFoundException("The employee with this id cannot be found.");

            if(!password.Validate(employee.HashedPassword, request.Password))
                throw new AuthenticationException("The password is incorrect.");

            return authenticationService.GenerateToken(employee);
        }

        public async Task UpdateEmployeeAsync(UpdateEmployeeDto dto)
        {
            var employee = await employeeRepository.GetByIdAsync(dto.Id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with id {dto.Id} not found.");

            employee.ToEmployeeEntity(dto);
            await employeeRepository.UpdateAsync(employee);
        }
    }
}
