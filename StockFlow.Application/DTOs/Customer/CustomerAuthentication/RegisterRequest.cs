using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.DTOs.Customer.CustomerAuthentication
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$",
            ErrorMessage = "Password must contain at least 8 characters, including an uppercase letter, lowercase letter, number, and special character.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postal code is required.")]
        [RegularExpression(
            @"^\d{4}\s?[A-HEJ-NPR-TV-Z]{2}$",
            ErrorMessage = "Please enter a valid Dutch postal code (e.g. 1234 AB).")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "House number is required.")]
        [RegularExpression(
            @"^\d+[A-Za-z]{0,3}$",
            ErrorMessage = "Please enter a valid house number (e.g. 12 or 12A).")]
        public string HouseNumber { get; set; } = string.Empty;
    }
}
