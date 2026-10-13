using StockFlow.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Employee
{
    public class UpdateEmployeeDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? NewPassword { get; set; }
        public EmployeeRoleDto Role { get; set; } = EmployeeRoleDto.None;
    }
}
