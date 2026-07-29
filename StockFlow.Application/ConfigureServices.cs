using Microsoft.Extensions.DependencyInjection;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Interfaces.ManagementApp;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Services;
using StockFlow.Application.Services.ManagementApp;
using StockFlow.Application.Services.ManagementApp.AuditLogs;
using StockFlow.Application.Services.ManagementApp.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.RegisterScopedServices();

            return services;
        }

        public static IServiceCollection RegisterScopedServices(this IServiceCollection services)
        {
            services.AddScoped<ICashRegisterLogsService, CashRegisterLogsService>();
            services.AddScoped<ICustomerLogsService, CustomerLogsService>();
            services.AddScoped<IStockLogsService, StockLogsService>();
            services.AddScoped<IProductBrandService, ProductBrandService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IShelfService, ShelfService>();
            services.AddScoped<ISalesService, SalesService>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            return services;
        }
    }
}
