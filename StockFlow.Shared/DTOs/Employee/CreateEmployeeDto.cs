using StockFlow.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Employee
{
    public class CreateEmployeeDto
    {
        public string FullName { get; set; }
        public string Password { get; set; } = string.Empty;
        public EmployeeRoleDto Role { get; set; } = EmployeeRoleDto.None;
    }
}
