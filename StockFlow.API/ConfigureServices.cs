using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Common;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StockFlow.API
{
    public static class ConfigureServices
    {
        public static IServiceCollection RegisterAPIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterAuthenticationServices(configuration);
            services.RegisterValidationResponses();

            return services;
        }

        public static IServiceCollection RegisterValidationResponses(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    string? message = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m));

                    return new BadRequestObjectResult(ApiResponse<object>.Fail(message ?? ApiMessages.InvalidRequest));
                };
            });

            return services;
        }

        public static IServiceCollection RegisterAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            string issuer = configuration.GetValue<string>("JWT:Issuer") ?? "";
            string key = configuration.GetValue<string>("JWT:Key") ?? "";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,

                        ValidateAudience = false,
                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents()
                    {
                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(ApiMessages.Forbidden));
                        },

                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));
                        }
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}
