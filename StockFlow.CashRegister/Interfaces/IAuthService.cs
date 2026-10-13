using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Employee.EmployeeAuthentication;

namespace StockFlow.CashRegister.Interfaces
{
	public interface IAuthService
	{
		Task<ApiResponse<Token>> LoginAsync(LoginRequest request);
		Task LogoutAsync();
	}
}
