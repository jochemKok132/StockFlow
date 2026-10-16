using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Employee
{
    public class CreateEmployeeDto
    {
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public string FullName { get; set; } = string.Empty;
        [MaxLength(FieldLimits.EmployeePassword, ErrorMessage = FieldLimits.TooLongMessage)]
        public string Password { get; set; } = string.Empty;
        public EmployeeRoleDto Role { get; set; } = EmployeeRoleDto.None;
    }
}
