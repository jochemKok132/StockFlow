using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.Stock;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class SalesSeeder
    {
        public static void UseSalesSeeder(this DbContext context)
        {
            if (!context.Set<Sales>().Any())
            {
                IEnumerable<Sales> SalesList = new List<Sales>()
                {
                    new Sales()
                    {
                        Id = new Guid("7c2e0d22-a35b-4656-b420-ed3e7398ca01"),
                        SaleName = "Summer Paint Sale",
                        Description = "Discount on selected paint products for the summer season.",
                        PercentageOff = 15,
                        SalesTags = new List<string>() { "paint", "summer", "discount" }
                    },
                    new Sales()
                    {
                        Id = new Guid("7c2e0d22-a35b-4656-b420-ed3e7398ca02"),
                        SaleName = "Hardware Clearance",
                        Description = "Clearance sale on selected hardware items.",
                        PercentageOff = 20,
                        SalesTags = new List<string>() { "hardware", "clearance" }
                    },
                    new Sales()
                    {
                        Id = new Guid("7c2e0d22-a35b-4656-b420-ed3e7398ca03"),
                        SaleName = "Sanitary Renovation Deal",
                        Description = "Discount on sanitary products for renovation projects.",
                        PercentageOff = 10,
                        SalesTags = new List<string>() { "sanitary", "renovation" }
                    },
                };
                context.Set<Sales>().AddRange(SalesList);
                context.SaveChanges();
            }
        }
    }
}