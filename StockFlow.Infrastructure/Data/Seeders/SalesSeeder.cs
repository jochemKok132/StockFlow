using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class SalesSeeder
    {
        public static void UseSalesSeeder(this DbContext context)
        {
            if (!context.Set<Sales>().Any())
            {
                IEnumerable<Sales> sales = new List<Sales>
                {
                    new Sales
                    {
                        Id = new Guid("9c4f0d22-a35b-4656-b420-000000000001"),
                        SaleName = "Paint Promotion",
                        Description = "Discount for paint products.",
                        PercentageOff = 10,
                        SalesTags = new List<string> { "paint" }
                    },
                    new Sales
                    {
                        Id = new Guid("9c4f0d22-a35b-4656-b420-000000000002"),
                        SaleName = "Hardware Promotion",
                        Description = "Discount for hardware products.",
                        PercentageOff = 5,
                        SalesTags = new List<string> { "hardware" }
                    }
                };

                context.Set<Sales>().AddRange(sales);
                context.SaveChanges();
            }

            var allSales = context.Set<Sales>().ToList();
            var products = context.Set<Product>().ToList();

            foreach (var sale in allSales)
            {
                var matchingProducts = products
                    .Where(product => product.SalesTags.Any(tag => sale.SalesTags.Contains(tag)))
                    .ToList();

                foreach (var product in matchingProducts)
                {
                    if (!sale.Products.Any(x => x.Id == product.Id))
                    {
                        sale.Products.Add(product);
                    }
                }
            }

            context.SaveChanges();
        }
    }
}
