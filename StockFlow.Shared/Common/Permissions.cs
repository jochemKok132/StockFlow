using System.Security.Claims;

namespace StockFlow.Shared.Common
{
    public static class Permissions
    {
        public const string Employees = "Admin,Manager,Support,ShiftLeader,ServiceDesk,Cashier,FloorWorker,Tester";
        public const string ManageStock = "Manager,Admin";
        public const string WriteOffStock = "Admin,Manager,ShiftLeader,ServiceDesk,Cashier,FloorWorker";
        public const string CashRegister = "Cashier,ServiceDesk,ShiftLeader,Manager,Admin,Tester";
        public const string CashRegisterLogs = "Cashier,ServiceDesk,ShiftLeader,Support,Manager,Admin,Tester";
        public const string StockLogs = "ServiceDesk,ShiftLeader,Support,Manager,Admin,Tester";
        public const string ProductHistory = StockLogs;
        public const string EmployeeLogs = "ShiftLeader,Support,Manager,Admin,Tester";
        public const string CustomerLogs = "Support,Manager,Admin,Tester";
        public const string ViewEmployees = "ShiftLeader,Support,Manager,Admin,Tester";
        public const string ManageEmployees = "Manager,Admin";
        public const string ViewCustomers = "ShiftLeader,Support,Manager,Admin,Tester";
        public const string ManageCustomers = "Manager,Admin";
        public const string ApproveMovingSales = "ShiftLeader,Manager,Admin";
        public const string ViewMovingSales = "ShiftLeader,Manager,Admin,Tester";
        public const string Statistics = "Support,Manager,Admin,Tester";
        public const string WriteLogs = "Manager,Admin";
        public const string HandleSupport = "Support,Admin";
        public const string ViewSupport = "Support,Admin,Tester";
        public const string HandleFeedback = "Admin";
        public const string ViewFeedback = "Admin,Tester";
        public const string RestoreDeleted = "Support,Admin";
        public const string ViewDeleted = "Support,Admin,Tester";

        public static bool Allows(this ClaimsPrincipal user, string roles) =>
            roles.Split(',').Any(user.IsInRole);

        public static bool CanOpen(this ClaimsPrincipal user, string manageRoles) =>
            user.Allows(manageRoles) || user.IsInRole(AccessRules.ViewOnlyRole);
    }
}
