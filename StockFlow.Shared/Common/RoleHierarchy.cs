using System.Security.Claims;
using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Shared.Common
{
    public static class RoleHierarchy
    {
        public const string OnlyLowerRoles = "You can only give out roles below your own.";
        public const string OnlyLowerEmployees = "You can only change employees below your own role.";
        public const string OnlyAdminAssignsTester = "Only an admin can give out the Tester role.";

        private static readonly EmployeeRoleDto[] Order =
        [
            EmployeeRoleDto.Admin,
            EmployeeRoleDto.Tester,
            EmployeeRoleDto.Manager,
            EmployeeRoleDto.Support,
            EmployeeRoleDto.ShiftLeader,
            EmployeeRoleDto.ServiceDesk,
            EmployeeRoleDto.Cashier,
            EmployeeRoleDto.FloorWorker,
        ];

        private static readonly EmployeeRoleDto[] Managers = [EmployeeRoleDto.Admin, EmployeeRoleDto.Manager];

        public static int Rank(EmployeeRoleDto role)
        {
            int index = Array.IndexOf(Order, role);
            return index < 0 ? int.MaxValue : index;
        }

        public static bool CanManageEmployees(EmployeeRoleDto actor) => Managers.Contains(actor);

        public static bool IsBelow(EmployeeRoleDto actor, EmployeeRoleDto target) =>
            Rank(actor) < Rank(target) && target != EmployeeRoleDto.None;

        public static bool CanManage(EmployeeRoleDto actor, EmployeeRoleDto target) =>
            CanManageEmployees(actor) && IsBelow(actor, target);

        public static IEnumerable<EmployeeRoleDto> AssignableRoles(EmployeeRoleDto actor) =>
            Order.Where(role => CanManage(actor, role));

        public static IReadOnlyList<EmployeeRoleDto> HiddenFrom(EmployeeRoleDto viewer) =>
            viewer == EmployeeRoleDto.Admin ? [] : [EmployeeRoleDto.Admin, EmployeeRoleDto.Tester];

        public static EmployeeRoleDto EmployeeRole(this ClaimsPrincipal user) =>
            Order.FirstOrDefault(role => user.IsInRole(role.ToString()), EmployeeRoleDto.None);
    }
}
