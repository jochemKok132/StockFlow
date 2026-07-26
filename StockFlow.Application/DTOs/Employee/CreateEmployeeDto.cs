using StockFlow.Application.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Employee
{
    public class CreateEmployeeDto
    {
        public required string FullName { get; set; }
        public required string EmployeeId { get; set; }
        public EmployeeRoleDto Role { get; set; } = EmployeeRoleDto.None;
    }
}
