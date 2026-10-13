
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Employee.EmployeeAuthentication;

namespace StockFlow.ManagementApp.Interfaces
{
	public interface IAuthService
	{
		Task<ApiResponse<Token>> LoginAsync(LoginRequest request);
		Task LogoutAsync();
	}
}
