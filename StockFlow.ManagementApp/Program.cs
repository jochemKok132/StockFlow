using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StockFlow.ManagementApp.Authentication;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.AuditLogs;
using StockFlow.ManagementApp.Interfaces.People;
using StockFlow.ManagementApp.Interfaces.Stock;
using StockFlow.ManagementApp.Services;
using StockFlow.ManagementApp.Services.AuditLogs;
using StockFlow.ManagementApp.Services.People;
using StockFlow.ManagementApp.Services.Stock;

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
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ISalesService, SalesService>();
            builder.Services.AddScoped<IShelfService, ShelfService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
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
