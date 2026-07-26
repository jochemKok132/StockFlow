using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.DTOs.Employee.EmployeeAuthentication
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Employee Id is required.")]
        public string EmployeeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
