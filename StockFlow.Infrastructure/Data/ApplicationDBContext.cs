using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Infrastructure.Data.Seeders;

namespace StockFlow.Infrastructure.Data
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<CashRegisterLogs> CashRegisterLogs { get; set; }
        public DbSet<CustomerLogs> CustomerLogs { get; set; }
        public DbSet<StockLogs> StockLogs { get; set; }
        public DbSet<EmployeeLogs> EmployeeLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductBrand>()
                   .HasMany(x => x.Products)
                   .WithOne(p => p.Brand)
                   .HasForeignKey(p => p.BrandId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                   .HasMany(x => x.Sales)
                   .WithMany(x => x.Products);

            builder.Entity<Shelf>()
                   .HasMany(x => x.Products)
                   .WithOne(p => p.Shelf)
                   .HasForeignKey(p => p.ShelfId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CashRegisterLogs>()
                   .HasOne(a => a.Employee)
                   .WithMany()
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CashRegisterLogs>()
                   .HasOne(a => a.Customer)
                   .WithMany()
                   .HasForeignKey(p => p.CustomerId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<CustomerLogs>()
                   .HasOne(x => x.Customer)
                   .WithMany()
                   .HasForeignKey(p => p.CustomerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StockLogs>()
                   .HasOne(a => a.Employee)
                   .WithMany()
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict); 
            
            builder.Entity<EmployeeLogs>()
                   .HasOne(a => a.Employee)
                   .WithMany()
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSeeding((context, _) =>
            {
                context.UseEmployeeSeeder();
                context.UseShelfSeeder();
                context.UseProductBrandSeeder();
                context.UseProductSeeder();
                context.UseSalesSeeder();
            });

            base.OnConfiguring(optionsBuilder);
        }
    }
}
