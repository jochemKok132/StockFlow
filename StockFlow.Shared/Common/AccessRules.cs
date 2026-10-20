using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Shared.Common
{
    public static class AccessRules
    {
        public const string ViewOnlyRole = nameof(EmployeeRoleDto.Tester);
        public const string AdminRole = nameof(EmployeeRoleDto.Admin);
        public const string ViewOnlyMessage = "This is a view-only account. Nothing was saved.";

        public static bool IsReadOnlyMethod(string method) =>
            method.Equals("GET", StringComparison.OrdinalIgnoreCase) ||
            method.Equals("HEAD", StringComparison.OrdinalIgnoreCase) ||
            method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase);
    }
}
