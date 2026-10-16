using System.Text.RegularExpressions;
using StockFlow.Shared.Common;

namespace StockFlow.Application.Validations.People
{
    public static class PasswordRules
    {
        public const int EmployeeMinLength = 4;
        public const int EmployeeMaxLength = FieldLimits.EmployeePassword;

        private static readonly Regex CustomerPattern = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$");

        public static void ValidateEmployeePassword(string? password, string field = "Password")
        {
            Guard.Required(password, field, EmployeeMaxLength);

            if (password!.Length < EmployeeMinLength || !password.All(char.IsDigit))
                throw new ArgumentException($"{field} must be {EmployeeMinLength} to {EmployeeMaxLength} digits.");
        }

        public static void ValidateCustomerPassword(string? password, string field = "Password")
        {
            Guard.Required(password, field, FieldLimits.CustomerPassword);

            if (!CustomerPattern.IsMatch(password!))
                throw new ArgumentException($"{field} must contain at least 8 characters, including an uppercase letter, lowercase letter, number, and special character.");
        }
    }
}
