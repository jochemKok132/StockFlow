
using StockFlow.Application.Common;
using StockFlow.Application.DTOs.Employee.EmployeeAuthentication;
using StockFlow.ManagementApp.Authentication;
using StockFlow.ManagementApp.Interfaces;

namespace StockFlow.ManagementApp.Services
{
	public class AuthService : IAuthService
	{
		private readonly AuthStateProvider _authStateProvider;
		private readonly IHttpService _httpService;

		public AuthService(AuthStateProvider authStateProvider, IHttpService httpService)
		{
			_authStateProvider = authStateProvider;
			_httpService = httpService;
		}

		public async Task<ApiResponse<Token>> LoginAsync(LoginRequest request)
		{
			var response = await _httpService.PostAsync<Token, LoginRequest>("Employee/Login", request);

			if (response.Succeeded && response.Value is not null)
				await _authStateProvider.MarkUserAsAuthenticatedAsync(response.Value.JWT);

			return response;
		}

		public async Task LogoutAsync()
		{
			await _authStateProvider.MarkUserAsLoggedOutAsync();
		}
	}
}
