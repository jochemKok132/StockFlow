using StockFlow.ManagementApp.Services.Scheduling;
using StockFlow.ManagementApp.Interfaces.Scheduling;
using StockFlow.ManagementApp.Services.Ordering;
using StockFlow.ManagementApp.Interfaces.Ordering;
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
            builder.Services.AddScoped<IEmployeeLogsService, EmployeeLogsService>();
            builder.Services.AddScoped<ICashRegisterLogsService, CashRegisterLogsService>();
            builder.Services.AddScoped<IReturnLogsService, ReturnLogsService>();
            builder.Services.AddScoped<ICustomerLogsService, CustomerLogsService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductBrandService, ProductBrandService>();
            builder.Services.AddScoped<ISalesService, SalesService>();
            builder.Services.AddScoped<IShelfService, ShelfService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<ILookupService, LookupService>();
            builder.Services.AddScoped<ConfirmService>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<IDeletedItemsService, DeletedItemsService>();
            builder.Services.AddScoped<IReleaseService, ReleaseService>();
            builder.Services.AddScoped<IStatisticsService, StatisticsService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
            builder.Services.AddScoped<ICustomerOrderService, CustomerOrderService>();
            builder.Services.AddScoped<IStockCountService, StockCountService>();
            builder.Services.AddScoped<IScheduleService, ScheduleService>();
            builder.Services.AddTransient<AuthHandler>();

            string apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7037/";
            var apiBaseAddress = new Uri(new Uri(builder.HostEnvironment.BaseAddress), apiBaseUrl);

            builder.Services.AddScoped(sp =>
            {
                var handler = sp.GetRequiredService<AuthHandler>();
                handler.InnerHandler = new HttpClientHandler();
                return new HttpClient(handler) { BaseAddress = apiBaseAddress };
            });

            await builder.Build().RunAsync();
        }
    }
}
