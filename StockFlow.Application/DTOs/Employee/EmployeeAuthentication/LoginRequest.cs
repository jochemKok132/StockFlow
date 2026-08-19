using System.ComponentModel.DataAnnotations;

namespace StockFlow.Application.DTOs.Employee.EmployeeAuthentication
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Employee Id is required and must be numerical.")]
        public string EmployeeId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required and must be numerical.")]
        public string Password { get; set; } = string.Empty;
    }
}
