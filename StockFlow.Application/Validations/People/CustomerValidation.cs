using System.Text.RegularExpressions;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Customer.CustomerAuthentication;
using CustomerLogin = StockFlow.Shared.DTOs.Customer.CustomerAuthentication.LoginRequest;

namespace StockFlow.Application.Validations.People
{
    public static class CustomerValidation
    {
        private static readonly Regex PostalCodePattern = new(@"^[1-9][0-9]{3}\s?[A-Za-z]{2}$");

        public static void Validate(this UpdateCustomerDto dto)
        {
            Guard.NotNull(dto, "Customer");
            Guard.NotEmpty(dto.Id, "Customer id");
            Guard.Required(dto.FullName, "Full name");
            Guard.Email(dto.Email);

            if (!string.IsNullOrWhiteSpace(dto.PostalCode) && !PostalCodePattern.IsMatch(dto.PostalCode.Trim()))
                throw new ArgumentException("Postal code must look like 1234 AB.");

            Guard.Optional(dto.HouseNumber, "House number", FieldLimits.HouseNumber);

            if (dto.SavedPoints < 0)
                throw new ArgumentException("Saved points can't be negative.");
        }

        public static void Validate(this ChangePasswordRequest request)
        {
            Guard.NotNull(request, "Change password");
            Guard.Required(request.CurrentPassword, "Current password", FieldLimits.CustomerPassword);
            PasswordRules.ValidateCustomerPassword(request.NewPassword, "New password");

            if (request.NewPassword != request.ConfirmPassword)
                throw new ArgumentException("The new passwords do not match.");

            if (request.NewPassword == request.CurrentPassword)
                throw new ArgumentException("The new password must be different from the current password.");
        }

        private static readonly Regex HouseNumberPattern = new(@"^\d+[A-Za-z]{0,3}$");

        public static void Validate(this RegisterRequest request)
        {
            Guard.NotNull(request, "Registration");
            Guard.Required(request.Name, "Name");

            if (request.Name.Trim().Length < 2)
                throw new ArgumentException("Name must be at least 2 characters.");

            Guard.Email(request.Email);
            PasswordRules.ValidateCustomerPassword(request.Password);

            if (request.Password != request.ConfirmPassword)
                throw new ArgumentException("The passwords do not match.");

            Guard.Required(request.PostalCode, "Postal code", FieldLimits.PostalCode);
            if (!PostalCodePattern.IsMatch(request.PostalCode.Trim()))
                throw new ArgumentException("Postal code must look like 1234 AB.");

            Guard.Required(request.HouseNumber, "House number", FieldLimits.HouseNumber);
            if (!HouseNumberPattern.IsMatch(request.HouseNumber.Trim()))
                throw new ArgumentException("House number must look like 12 or 12A.");
        }

        public static string NormalizePostalCode(string postalCode)
        {
            var compact = postalCode.Replace(" ", "").ToUpperInvariant();
            return $"{compact[..4]} {compact[4..]}";
        }

        public static void Validate(this CustomerLogin request)
        {
            Guard.NotNull(request, "Login");
            Guard.Email(request.Email);
            Guard.Required(request.Password, "Password");
        }
    }
}
