using StockFlow.Shared.Common;
using StockFlow.Domain.Entities.Scheduling;
using StockFlow.Domain.Entities.Ordering;
using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Infrastructure.Data.Seeders;
using StockFlow.Domain.Entities.CashRegister;
using StockFlow.Domain.Entities.Support;

namespace StockFlow.Infrastructure.Data
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAvailability> EmployeeAvailability { get; set; }
        public DbSet<ShiftTemplate> ShiftTemplates { get; set; }
        public DbSet<ScheduleWeek> ScheduleWeeks { get; set; }
        public DbSet<ScheduledShift> ScheduledShifts { get; set; }
        public DbSet<ScheduleRequest> ScheduleRequests { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<CashRegisterLogs> CashRegisterLogs { get; set; }
        public DbSet<CustomerLogs> CustomerLogs { get; set; }
        public DbSet<StockLogs> StockLogs { get; set; }
        public DbSet<EmployeeLogs> EmployeeLogs { get; set; }
        public DbSet<RegisterItemDetail> RegisterItemDetails { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<ReturnLogs> ReturnLogs { get; set; }
        public DbSet<ReturnLogLine> ReturnLogLines { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<StockBatch> StockBatches { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<CustomerOrderLine> CustomerOrderLines { get; set; }
        public DbSet<StockCount> StockCounts { get; set; }
        public DbSet<StockCountLine> StockCountLines { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureOrdering(builder);
            ConfigureReturnLogs(builder);
            ConfigureScheduling(builder);

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
                   .HasIndex(e => e.ReceiptId)
                   .IsUnique();

            builder.Entity<Ticket>()
                   .HasIndex(t => t.TicketNumber)
                   .IsUnique();

            builder.Entity<Ticket>()
                   .HasOne(t => t.CreatedBy)
                   .WithMany()
                   .HasForeignKey(t => t.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                   .HasOne(t => t.HandledBy)
                   .WithMany()
                   .HasForeignKey(t => t.HandledById)
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

            builder.Entity<CashRegisterLogs>()
                    .HasMany(a => a.ProductsSold)
                    .WithOne()
                    .HasForeignKey(a => a.CashregisterLogId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RegisterItemDetail>()
                    .HasOne(e => e.Sale)
                    .WithMany()
                    .HasForeignKey(p => p.SalesId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RegisterItemDetail>()
                    .HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(p => p.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

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

        private static void ConfigureScheduling(ModelBuilder builder)
        {
            builder.Entity<Employee>()
                   .Property(e => e.ContractHours)
                   .HasDefaultValue(0);

            builder.Entity<EmployeeAvailability>()
                   .HasOne(a => a.Employee)
                   .WithMany(e => e.Availability)
                   .HasForeignKey(a => a.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EmployeeAvailability>()
                   .HasIndex(a => new { a.EmployeeId, a.Day })
                   .IsUnique();

            builder.Entity<ShiftTemplate>()
                   .Property(t => t.Name)
                   .HasMaxLength(FieldLimits.Name);

            builder.Entity<ScheduleWeek>()
                   .HasIndex(w => w.WeekStart)
                   .IsUnique();

            builder.Entity<ScheduleWeek>()
                   .HasOne(w => w.ConfirmedBy)
                   .WithMany()
                   .HasForeignKey(w => w.ConfirmedById)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ScheduleWeek>()
                   .HasMany(w => w.Shifts)
                   .WithOne(s => s.ScheduleWeek)
                   .HasForeignKey(s => s.ScheduleWeekId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ScheduledShift>()
                   .HasOne(s => s.Employee)
                   .WithMany()
                   .HasForeignKey(s => s.EmployeeId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ScheduledShift>()
                   .HasIndex(s => new { s.EmployeeId, s.Date });

            builder.Entity<ScheduleRequest>()
                   .HasIndex(r => r.RequestNumber)
                   .IsUnique();

            builder.Entity<ScheduleRequest>()
                   .Property(r => r.Reason)
                   .HasMaxLength(FieldLimits.Reason);

            builder.Entity<ScheduleRequest>()
                   .Property(r => r.ReviewNote)
                   .HasMaxLength(FieldLimits.Reason);

            builder.Entity<ScheduleRequest>()
                   .HasOne(r => r.Employee)
                   .WithMany()
                   .HasForeignKey(r => r.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ScheduleRequest>()
                   .HasOne(r => r.TargetEmployee)
                   .WithMany()
                   .HasForeignKey(r => r.TargetEmployeeId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ScheduleRequest>()
                   .HasOne(r => r.ReviewedBy)
                   .WithMany()
                   .HasForeignKey(r => r.ReviewedById)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ScheduleRequest>()
                   .HasOne(r => r.Shift)
                   .WithMany()
                   .HasForeignKey(r => r.ShiftId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ScheduleRequest>()
                   .HasOne(r => r.TargetShift)
                   .WithMany()
                   .HasForeignKey(r => r.TargetShiftId)
                   .OnDelete(DeleteBehavior.SetNull);
        }

        private static void ConfigureReturnLogs(ModelBuilder builder)
        {
            builder.Entity<ReturnLogs>()
                   .HasOne(r => r.CashRegisterLog)
                   .WithMany()
                   .HasForeignKey(r => r.CashRegisterLogId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ReturnLogs>()
                   .HasOne(r => r.Employee)
                   .WithMany()
                   .HasForeignKey(r => r.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ReturnLogs>()
                   .HasOne(r => r.Customer)
                   .WithMany()
                   .HasForeignKey(r => r.CustomerId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ReturnLogs>()
                   .HasIndex(r => r.ReceiptId);

            builder.Entity<ReturnLogs>()
                   .HasMany(r => r.Lines)
                   .WithOne()
                   .HasForeignKey(l => l.ReturnLogId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ReturnLogLine>()
                   .HasOne(l => l.Product)
                   .WithMany()
                   .HasForeignKey(l => l.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureOrdering(ModelBuilder builder)
        {
            builder.Entity<Supplier>()
                   .HasMany(s => s.Products)
                   .WithOne(p => p.Supplier)
                   .HasForeignKey(p => p.SupplierId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                   .HasMany(p => p.Prices)
                   .WithOne(p => p.Product)
                   .HasForeignKey(p => p.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductPrice>()
                   .HasOne(p => p.CreatedBy)
                   .WithMany()
                   .HasForeignKey(p => p.CreatedById)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ProductPrice>()
                   .HasIndex(p => new { p.ProductId, p.ValidFrom });

            builder.Entity<Product>()
                   .HasMany(p => p.Batches)
                   .WithOne(b => b.Product)
                   .HasForeignKey(b => b.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PurchaseOrder>()
                   .HasIndex(o => o.OrderNumber)
                   .IsUnique();

            builder.Entity<PurchaseOrder>()
                   .HasOne(o => o.Supplier)
                   .WithMany()
                   .HasForeignKey(o => o.SupplierId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PurchaseOrder>()
                   .HasOne(o => o.ReceivedBy)
                   .WithMany()
                   .HasForeignKey(o => o.ReceivedById)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<PurchaseOrder>()
                   .HasMany(o => o.Lines)
                   .WithOne(l => l.PurchaseOrder)
                   .HasForeignKey(l => l.PurchaseOrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PurchaseOrderLine>()
                   .HasOne(l => l.Product)
                   .WithMany()
                   .HasForeignKey(l => l.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerOrder>()
                   .HasIndex(o => o.OrderNumber)
                   .IsUnique();

            builder.Entity<CustomerOrder>()
                   .HasOne(o => o.Customer)
                   .WithMany()
                   .HasForeignKey(o => o.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerOrder>()
                   .HasOne(o => o.CreatedBy)
                   .WithMany()
                   .HasForeignKey(o => o.CreatedById)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<CustomerOrder>()
                   .HasMany(o => o.Lines)
                   .WithOne(l => l.CustomerOrder)
                   .HasForeignKey(l => l.CustomerOrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CustomerOrderLine>()
                   .HasOne(l => l.Product)
                   .WithMany()
                   .HasForeignKey(l => l.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StockCount>()
                   .HasIndex(c => c.CountNumber)
                   .IsUnique();

            builder.Entity<StockCount>()
                   .HasOne(c => c.Shelf)
                   .WithMany()
                   .HasForeignKey(c => c.ShelfId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StockCount>()
                   .HasOne(c => c.CreatedBy)
                   .WithMany()
                   .HasForeignKey(c => c.CreatedById)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<StockCount>()
                   .HasOne(c => c.CompletedBy)
                   .WithMany()
                   .HasForeignKey(c => c.CompletedById)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<StockCount>()
                   .HasMany(c => c.Lines)
                   .WithOne(l => l.StockCount)
                   .HasForeignKey(l => l.StockCountId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<StockCountLine>()
                   .HasOne(l => l.Product)
                   .WithMany()
                   .HasForeignKey(l => l.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSeeding((context, _) =>
            {
                context.UseEmployeeSeeder();
                context.UseCustomerSeeder();
                context.UseShelfSeeder();
                context.UseSupplierSeeder();
                context.UseProductBrandSeeder();
                context.UseProductSeeder();
                context.UseSalesSeeder();
                context.UseCashRegisterLogSeeder();
                context.UseCustomerOrderSeeder();
                context.UsePurchaseOrderSeeder();
                context.UseStockCountSeeder();
                context.UseTicketSeeder();
                context.UseScheduleSeeder();
                context.UseLogSeeder();
            });

            base.OnConfiguring(optionsBuilder);
        }
    }
}
