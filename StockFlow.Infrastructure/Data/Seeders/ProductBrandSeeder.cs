using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.Stock;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class ProductBrandSeeder
    {
        public static void UseProductBrandSeeder(this DbContext context)
        {
            if (!context.Set<ProductBrand>().Any())
            {
                IEnumerable<ProductBrand> ProductBrands = new List<ProductBrand>()
                {
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000001"),
                        BrandName = "Flexa Verf"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000002"),
                        BrandName = "Sigma Coatings"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000003"),
                        BrandName = "Histor"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000004"),
                        BrandName = "Sikkens"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000005"),
                        BrandName = "Levis Verf"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000006"),
                        BrandName = "GAMMA"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000007"),
                        BrandName = "Bosch"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000008"),
                        BrandName = "Makita"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000009"),
                        BrandName = "Fischer"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000a"),
                        BrandName = "Facom"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000b"),
                        BrandName = "Legrand"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000c"),
                        BrandName = "Philips"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000d"),
                        BrandName = "Schneider Electric"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000e"),
                        BrandName = "Niko"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-00000000000f"),
                        BrandName = "ABB"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000010"),
                        BrandName = "Praxis Bouwmaterialen"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000011"),
                        BrandName = "Kronoply"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000012"),
                        BrandName = "Egger"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000013"),
                        BrandName = "Steico"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000014"),
                        BrandName = "Timberpro"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000015"),
                        BrandName = "Hansgrohe"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000016"),
                        BrandName = "Villeroy & Boch"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000017"),
                        BrandName = "Grohe"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000018"),
                        BrandName = "Duravit"
                    },
                    new ProductBrand()
                    {
                        Id = new Guid("6b1d0d22-a35b-4656-b420-000000000019"),
                        BrandName = "Geberit"
                    },
                };
                context.Set<ProductBrand>().AddRange(ProductBrands);
                context.SaveChanges();
            }
        }
    }
}