using StockFlow.Application.Common;
using StockFlow.Application.DTOs.Employee.EmployeeAuthentication;

namespace StockFlow.CashRegister.Interfaces
{
	public interface IAuthService
	{
		Task<ApiResponse<Token>> LoginAsync(LoginRequest request);
		Task LogoutAsync();
	}
}
