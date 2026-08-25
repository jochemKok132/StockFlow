using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StockFlow.ManagementApp.Authentication;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.AuditLogs;
using StockFlow.ManagementApp.Services;
using StockFlow.ManagementApp.Services.AuditLogs;

namespace StockFlow.ManagementApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<SessionStorageService>();
            builder.Services.AddScoped<AuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IHttpService, HttpService>();
            builder.Services.AddScoped<IStockLogsService, StockLogsService>();
            builder.Services.AddScoped<ICashRegisterLogsService, CashRegisterLogsService>();
            builder.Services.AddScoped<ICustomerLogsService, CustomerLogsService>();
            builder.Services.AddTransient<AuthHandler>();

            builder.Services.AddScoped(sp =>
            {
                var handler = sp.GetRequiredService<AuthHandler>();
                handler.InnerHandler = new HttpClientHandler();
                return new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7037/") };
            });

            await builder.Build().RunAsync();
        }
    }
}
