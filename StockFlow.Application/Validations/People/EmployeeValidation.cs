using StockFlow.Shared.DTOs.Employee;
using StockFlow.Shared.DTOs.Enums;
using EmployeeLogin = StockFlow.Shared.DTOs.Employee.EmployeeAuthentication.LoginRequest;

namespace StockFlow.Application.Validations.People
{
    public static class EmployeeValidation
    {
        public static void Validate(this CreateEmployeeDto dto)
        {
            Guard.NotNull(dto, "Employee");
            Guard.Required(dto.FullName, "Full name");
            ValidateRole(dto.Role);
            PasswordRules.ValidateEmployeePassword(dto.Password);
        }

        public static void Validate(this UpdateEmployeeDto dto)
        {
            Guard.NotNull(dto, "Employee");
            Guard.NotEmpty(dto.Id, "Employee id");
            Guard.Required(dto.FullName, "Full name");
            ValidateRole(dto.Role);

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
                PasswordRules.ValidateEmployeePassword(dto.NewPassword, "New password");
        }

        public static void Validate(this EmployeeLogin request)
        {
            Guard.NotNull(request, "Login");
            Guard.Required(request.EmployeeId, "Employee Id", 6);
            Guard.Required(request.Password, "Password");

            if (!request.EmployeeId.All(char.IsDigit))
                throw new ArgumentException("Employee Id can only contain numbers.");
        }

        private static void ValidateRole(EmployeeRoleDto role)
        {
            if (!Enum.IsDefined(role) || role == EmployeeRoleDto.None)
                throw new ArgumentException("Choose a valid role.");
        }
    }
}
