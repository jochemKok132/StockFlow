using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.Stock;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class SupplierSeeder
    {
        public static readonly Guid PaintSupplierId = new("5a9e0d22-a35b-4656-b420-000000000002");
        public static readonly Guid SanitarySupplierId = new("5a9e0d22-a35b-4656-b420-000000000003");
        public static readonly Guid ElectroSupplierId = new("5a9e0d22-a35b-4656-b420-000000000004");
        public static readonly Guid TimberSupplierId = new("5a9e0d22-a35b-4656-b420-000000000005");

        public static void UseSupplierSeeder(this DbContext context)
        {
            if (context.Set<Product>().Any() || context.Set<Supplier>().Any(s => s.Id != Supplier.DefaultSupplierId))
                return;

            context.Set<Supplier>().AddRange(
                new Supplier
                {
                    Id = PaintSupplierId,
                    SupplierName = "ColorPro Paint Supply",
                    ContactName = "Inge Verhoef",
                    Email = "orders@colorpro.example",
                    Phone = "0314-123456",
                    Website = "https://colorpro.example",
                    OrderDay = DayOfWeek.Tuesday,
                    LeadTimeDays = 2,
                },
                new Supplier
                {
                    Id = SanitarySupplierId,
                    SupplierName = "Sanitary Direct",
                    ContactName = "Ruben Lammers",
                    Email = "orders@sanitarydirect.example",
                    Phone = "0315-654321",
                    Website = "https://sanitarydirect.example",
                    OrderDay = DayOfWeek.Thursday,
                    LeadTimeDays = 3,
                },
                new Supplier
                {
                    Id = ElectroSupplierId,
                    SupplierName = "ElectroParts Supply",
                    ContactName = "Mehmet Yilmaz",
                    Email = "orders@electroparts.example",
                    Phone = "0316-246810",
                    Website = "https://electroparts.example",
                    OrderDay = DayOfWeek.Wednesday,
                    LeadTimeDays = 1,
                },
                new Supplier
                {
                    Id = TimberSupplierId,
                    SupplierName = "TimberTrade",
                    ContactName = "Anouk Hermsen",
                    Email = "orders@timbertrade.example",
                    Phone = "0317-135790",
                    Website = "https://timbertrade.example",
                    OrderDay = DayOfWeek.Friday,
                    LeadTimeDays = 5,
                });
            context.SaveChanges();
        }
    }
}
