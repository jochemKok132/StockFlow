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
        public const string CustomerRole = "Customer";

        public DTOs.Employee.EmployeeAuthentication.Token GenerateToken(Employee user)
        {
            var (jwt, expiresOn) = CreateToken(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? ""),
                new Claim(ClaimTypes.Thumbprint, user.EmployeeId ?? ""),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            });

            return new DTOs.Employee.EmployeeAuthentication.Token() { JWT = jwt, ExpiresOn = expiresOn };
        }

        public DTOs.Customer.CustomerAuthentication.Token GenerateToken(Customer user)
        {
            var (jwt, expiresOn) = CreateToken(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Thumbprint, user.CustomerId ?? ""),
                new Claim(ClaimTypes.Role, CustomerRole)
            });

            return new DTOs.Customer.CustomerAuthentication.Token() { JWT = jwt, ExpiresOn = expiresOn };
        }

        private (string jwt, DateTime expiresOn) CreateToken(List<Claim> claimList)
        {
            ClaimsIdentity claims = new ClaimsIdentity(claimList);

            string? jwtKey = configuration["JWT:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new InvalidOperationException("The JWT secret key is empty!");

            string? issuer = configuration["JWT:Issuer"];
            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("The JWT issuer is empty!");

            string? expiryConfig = configuration["JWT:ExpiryMinutes"];
            if (string.IsNullOrWhiteSpace(expiryConfig))
                throw new InvalidOperationException("The JWT expiry minutes is empty!");

            if (!double.TryParse(expiryConfig, out double expiryMinutes) || expiryMinutes <= 0)
                throw new InvalidOperationException("The JWT expiry minutes is invalid!");

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
            return (token, expiresOn);
        }
    }
}
