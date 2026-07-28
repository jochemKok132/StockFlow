using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockFlow.Application.Interfaces.Helpers;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Services.ManagementApp.AuditLogs;
using StockFlow.Application.Services.ManagementApp.Stock;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Helpers;
using StockFlow.Infrastructure.Repositories.EfRepositories;
using StockFlow.Infrastructure.Repositories.Stock;

namespace StockFlow.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration, string root)
        {
            services.RegisterDatabaseServices(configuration, root);
            services.AddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));
            services.AddScoped(typeof(IEfCreatableRepository<>), typeof(EfCreatableRepository<>));
            services.AddScoped(typeof(IEfUpdatableRepository<>), typeof(EfUpdatableRepository<>));
            services.AddScoped(typeof(IEfSoftDeletableRepository<>), typeof(EfSoftDeletableRepository<>));
            services.AddScoped<IPassword, Password>();
            services.AddScoped<ICashRegisterLogsRepository, CashRegisterLogsRepository>();
            services.AddScoped<ICustomerLogsRepository, CustomerLogsRepository>();
            services.AddScoped<IStockLogsRepository, StockLogsRepository>();
            services.AddScoped<IProductBrandRepository, ProductBrandRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IShelfRepository, ShelfRepository>();
            services.AddScoped<ISalesRepository, SalesRepository>();

            return services;
        }

        public static IServiceCollection RegisterDatabaseServices(this IServiceCollection services, IConfiguration configuration, string root)
        {
            string folder = Path.GetFullPath(Path.Combine(root, "..", "Infrastructure", "Data"));

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string dataSource = configuration["DataSource"] ?? "app.db";
            string connectionString = $"Data Source={Path.Combine(folder, dataSource)}";

            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlite(connectionString));

            return services;
        }
    }
}
