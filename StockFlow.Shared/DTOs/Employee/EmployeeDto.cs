using StockFlow.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Employee
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required string EmployeeId { get; set; }
        public required string FullName { get; set; }
        public EmployeeRoleDto Role { get; set; } = EmployeeRoleDto.None;
    }
}
