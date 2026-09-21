using StockFlow.Application.DTOs.Enums;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.Enums
{
    public static class EmployeeRoleMapping
    {
        public static EmployeeRole ToDomain(this EmployeeRoleDto role)
        {
            return role switch
            {
                EmployeeRoleDto.Admin => EmployeeRole.Admin,
                EmployeeRoleDto.Manager => EmployeeRole.Manager,
                EmployeeRoleDto.ShiftLeader => EmployeeRole.ShiftLeader,
                EmployeeRoleDto.ServiceDesk => EmployeeRole.ServiceDesk,
                EmployeeRoleDto.Cashier => EmployeeRole.Cashier,
                EmployeeRoleDto.FloorWorker => EmployeeRole.FloorWorker,
                EmployeeRoleDto.None => EmployeeRole.None,
                _ => throw new ArgumentOutOfRangeException(nameof(role))
            };
        }

        public static EmployeeRoleDto ToDto(this EmployeeRole role)
        {
            return role switch
            {
                EmployeeRole.Admin => EmployeeRoleDto.Admin,
                EmployeeRole.Manager => EmployeeRoleDto.Manager,
                EmployeeRole.ShiftLeader => EmployeeRoleDto.ShiftLeader,
                EmployeeRole.ServiceDesk => EmployeeRoleDto.ServiceDesk,
                EmployeeRole.Cashier => EmployeeRoleDto.Cashier,
                EmployeeRole.FloorWorker => EmployeeRoleDto.FloorWorker,
                EmployeeRole.None => EmployeeRoleDto.None,
                _ => throw new ArgumentOutOfRangeException(nameof(role))
            };
        }
    }
}
