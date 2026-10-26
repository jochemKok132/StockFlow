using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class SalesSeeder
    {
        public static void UseSalesSeeder(this DbContext context)
        {
            if (!context.Set<Sales>().Any(s => s.SaleType != SaleType.Moving))
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
                    },
                    new Sales
                    {
                        Id = new Guid("9c4f0d22-a35b-4656-b420-000000000003"),
                        SaleName = "Sanitary 2+1",
                        Description = "Buy two sanitary products, get the third one free.",
                        SaleType = SaleType.MultiBuy,
                        BuyAmount = 2,
                        FreeAmount = 1,
                        StartDate = DateTime.Today.AddDays(-10),
                        EndDate = DateTime.Today.AddDays(20),
                        SalesTags = new List<string> { "sanitary" }
                    },
                    new Sales
                    {
                        Id = new Guid("9c4f0d22-a35b-4656-b420-000000000004"),
                        SaleName = "Electronics second half price",
                        Description = "The second electronics product is half price.",
                        SaleType = SaleType.SecondItemHalfPrice,
                        StartDate = DateTime.Today.AddDays(-5),
                        EndDate = DateTime.Today.AddDays(10),
                        SalesTags = new List<string> { "electronics" }
                    },
                    new Sales
                    {
                        Id = new Guid("9c4f0d22-a35b-4656-b420-000000000005"),
                        SaleName = "Wood bundle",
                        Description = "Three wood products for 25 euro.",
                        SaleType = SaleType.Bundle,
                        BuyAmount = 3,
                        BundlePrice = 25m,
                        StartDate = DateTime.Today.AddDays(7),
                        EndDate = DateTime.Today.AddDays(21),
                        SalesTags = new List<string> { "wood" }
                    },
                    new Sales
                    {
                        Id = new Guid("9c4f0d22-a35b-4656-b420-000000000006"),
                        SaleName = "Summer Paint Sale",
                        Description = "15 % off all paint during the summer.",
                        PercentageOff = 15,
                        StartDate = DateTime.Today.AddDays(-90),
                        EndDate = DateTime.Today.AddDays(-60),
                        SalesTags = new List<string> { "paint" }
                    }
                };

                context.Set<Sales>().AddRange(sales);
                context.SaveChanges();
            }

            var allSales = context.Set<Sales>().Include(s => s.Products).ToList();
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
