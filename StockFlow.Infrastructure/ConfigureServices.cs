using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockFlow.Infrastructure.Repositories.EfRepositories;

namespace StockFlow.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration, string root)
        {
            services.RegisterDatabaseServices(configuration, root);
            services.AddScoped(typeof(IEfRepository<>), typeof(EfRepository<>));

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
