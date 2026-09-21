using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using StockFlow.Application.DTOs.Customer.CustomerAuthentication;
using StockFlow.Application.DTOs.Employee.EmployeeAuthentication;
using StockFlow.Application.Interfaces;
using StockFlow.Domain.Entities.People;
using System.Security.Claims;
using System.Text;

namespace StockFlow.Application.Services
{
    public class AuthenticationService(IConfiguration configuration) : IAuthenticationService
    {
        public DTOs.Employee.EmployeeAuthentication.Token GenerateToken(Employee user)
        {
            ClaimsIdentity claims = new ClaimsIdentity(
                new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName ?? ""),
                    new Claim(ClaimTypes.Thumbprint, user.EmployeeId ?? ""),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }
            );

            string? jwtKey = configuration["JWT:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new ArgumentException("The JWT secret key is empty!");

            string? issuer = configuration["JWT:Issuer"];
            if (string.IsNullOrWhiteSpace(issuer))
                throw new ArgumentException("The JWT issuer is empty!");

            string? expiryConfig = configuration["JWT:ExpiryMinutes"];
            if (string.IsNullOrWhiteSpace(expiryConfig))
                throw new ArgumentException("The JWT expiry minutes is empty!");

            if (!double.TryParse(expiryConfig, out double expiryMinutes) || expiryMinutes <= 0)
                throw new ArgumentException("The JWT expiry minutes is invalid!");

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            DateTime expiresOn = DateTime.UtcNow.AddMinutes(expiryMinutes);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Issuer = issuer,
                Subject = claims,
                Expires = expiresOn,
                SigningCredentials = credentials
            };

            string token = new JsonWebTokenHandler().CreateToken(descriptor);
            return new DTOs.Employee.EmployeeAuthentication.Token() { JWT = token, ExpiresOn = expiresOn };
        }

        public DTOs.Customer.CustomerAuthentication.Token GenerateToken(Customer user)
        {
            throw new NotImplementedException();
        }
    }
}
