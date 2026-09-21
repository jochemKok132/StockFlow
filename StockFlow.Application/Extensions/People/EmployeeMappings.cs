using StockFlow.Application.DTOs.Employee;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Domain.Entities.People;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.People
{
    public static class EmployeeMappings
    {
        public static EmployeeDto ToEmployeeDto(this Employee employee)
        {
            return new EmployeeDto()
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                Id = employee.Id,
                Role = employee.Role.ToDto(),
                UpdatedAt = employee.UpdatedAt,
                CreatedAt = employee.CreatedAt,
            };
        }
        public static Employee ToEmployeeEntity(this CreateEmployeeDto dto)
        {
            return new Employee()
            {
                Id = Guid.NewGuid(),
                EmployeeId = null!,
                FullName = dto.FullName,
                Role = dto.Role.ToDomain(),
            };
        }
        public static void ToEmployeeEntity(this Employee entity, UpdateEmployeeDto dto)
        {
            entity.FullName = dto.FullName;
            if(!string.IsNullOrWhiteSpace(dto.HashedPassword)) entity.HashedPassword = dto.HashedPassword;
            entity.Role = dto.Role.ToDomain();
        }
    }
}
