using System.Security.Claims;

namespace StockFlow.Shared.Common
{
    public static class Permissions
    {
        public const string Employees = "Admin,Manager,ShiftLeader,ServiceDesk,Cashier,FloorWorker,Tester";
        public const string ManageStock = "Manager,Admin";
        public const string CashRegister = "Cashier,ServiceDesk,ShiftLeader,Manager,Admin,Tester";
        public const string CashRegisterLogs = CashRegister;
        public const string StockLogs = "ServiceDesk,ShiftLeader,Manager,Admin,Tester";
        public const string ProductHistory = StockLogs;
        public const string EmployeeLogs = "ShiftLeader,Manager,Admin,Tester";
        public const string CustomerLogs = "Manager,Admin,Tester";
        public const string ViewEmployees = "ShiftLeader,Manager,Admin,Tester";
        public const string ManageEmployees = "Manager,Admin";
        public const string ViewCustomers = "ShiftLeader,Manager,Admin,Tester";
        public const string ManageCustomers = "Manager,Admin";
        public const string ApproveMovingSales = "ShiftLeader,Manager,Admin";
        public const string ViewMovingSales = "ShiftLeader,Manager,Admin,Tester";
        public const string Statistics = "Manager,Admin,Tester";
        public const string WriteLogs = "Manager,Admin";

        public static bool Allows(this ClaimsPrincipal user, string roles) =>
            roles.Split(',').Any(user.IsInRole);

        public static bool CanOpen(this ClaimsPrincipal user, string manageRoles) =>
            user.Allows(manageRoles) || user.IsInRole(AccessRules.ViewOnlyRole);
    }
}
