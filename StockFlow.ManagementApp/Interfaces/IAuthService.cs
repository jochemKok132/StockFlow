
using StockFlow.Application.Common;
using StockFlow.Application.DTOs.Employee.EmployeeAuthentication;

namespace StockFlow.ManagementApp.Interfaces
{
	public interface IAuthService
	{
		Task<ApiResponse<Token>> LoginAsync(LoginRequest request);
		Task LogoutAsync();
	}
}
