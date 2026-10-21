using System.Security.Claims;
using StockFlow.Shared.Common;

namespace StockFlow.API
{
    public static class ApiMessages
    {
        public const string Unexpected = "An unexpected error occurred.";
        public const string NotLoggedIn = "You are not logged in.";
        public const string Forbidden = "You don't have access to this function.";
        public const string InvalidRequest = "The request is invalid.";
    }

    public static class AuthRoles
    {
        public const string Employees = Permissions.Employees;
        public const string Customer = "Customer";
    }

    public static class ClaimsPrincipalExtensions
    {
        public static bool TryGetUserId(this ClaimsPrincipal user, out Guid userId) =>
            Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}
