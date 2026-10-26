using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.Stock;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class ProductSeeder
    {
        public static void UseProductSeeder(this DbContext context)
        {
            if (!context.Set<Product>().Any())
            {
                Guid flexaBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000001");
                Guid sigmaBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000002");
                Guid historBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000003");
                Guid sikkensBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000004");
                Guid levisBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000005");
                Guid gammaBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000006");
                Guid boschBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000007");
                Guid makitaBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000008");
                Guid fischerBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000009");
                Guid facomBrand = new Guid("6b1d0d22-a35b-4656-b420-00000000000a");
                Guid legrandBrand = new Guid("6b1d0d22-a35b-4656-b420-00000000000b");
                Guid philipsBrand = new Guid("6b1d0d22-a35b-4656-b420-00000000000c");
                Guid schneiderBrand = new Guid("6b1d0d22-a35b-4656-b420-00000000000d");
                Guid nikoBrand = new Guid("6b1d0d22-a35b-4656-b420-00000000000e");
                Guid abbBrand = new Guid("6b1d0d22-a35b-4656-b420-00000000000f");
                Guid praxisBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000010");
                Guid kronoplyBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000011");
                Guid eggerBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000012");
                Guid steicoBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000013");
                Guid timberproBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000014");
                Guid hansgroheBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000015");
                Guid villeroyBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000016");
                Guid groheBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000017");
                Guid duravitBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000018");
                Guid geberitBrand = new Guid("6b1d0d22-a35b-4656-b420-000000000019");

                Guid flexaShelf = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde3");
                Guid sigmaShelf = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde4");
                Guid historShelf = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde5");
                Guid screwsShelf = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde6");
                Guid nailsShelf = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde7");
                Guid locksShelf = new Guid("55ad0d22-a35b-4656-b420-ed3e6398cde5");
                Guid socketsShelf = new Guid("55ad0d22-a35b-4656-b420-ed3e5398cde5");
                Guid plugsShelf = new Guid("55ad0d22-a35b-4656-b420-ed3e4398cde5");
                Guid wiresShelf = new Guid("55ad0d22-a35b-4656-b420-ed3e3398cde5");
                Guid planksShelf = new Guid("55ad0d22-a35b-4656-b430-ed3e3398cde5");
                Guid beamsShelf = new Guid("55ad0d22-a35b-4656-b440-ed3e3398cde5");
                Guid impregnatedWoodShelf = new Guid("55ad0d22-a35b-4656-b450-ed3e3398cde5");
                Guid showersShelf = new Guid("55ad0d22-a35b-4656-b530-ed3e3398cde5");
                Guid waterTapShelf = new Guid("55ad0d22-a35b-4656-b630-ed3e3398cde5");
                Guid showerMatsShelf = new Guid("55ad0d22-a35b-4656-b330-ed3e3398cde5");

                IEnumerable<Product> Products = new List<Product>()
                {
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000001"),
                        ProductName = "Flexa Wall Paint White 1L",
                        Description = "Wall Paint White 1L by Flexa.",
                        Barcode = 500000001,
                        Stock = 34,
                        Price = 15.49m,
                        Location = 1001,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = flexaBrand,
                        ShelfId = flexaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000002"),
                        ProductName = "Sigma Coatings Wall Paint Grey 1L",
                        Description = "Wall Paint Grey 1L by Sigma Coatings.",
                        Barcode = 500000002,
                        Stock = 41,
                        Price = 16.49m,
                        Location = 1002,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sigmaBrand,
                        ShelfId = flexaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000003"),
                        ProductName = "Histor Wall Paint Black 1L",
                        Description = "Wall Paint Black 1L by Histor.",
                        Barcode = 500000003,
                        Stock = 48,
                        Price = 12.49m,
                        Location = 1003,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = historBrand,
                        ShelfId = flexaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000004"),
                        ProductName = "Sikkens Wall Paint Beige 1L",
                        Description = "Wall Paint Beige 1L by Sikkens.",
                        Barcode = 500000004,
                        Stock = 55,
                        Price = 13.49m,
                        Location = 1004,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sikkensBrand,
                        ShelfId = flexaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000005"),
                        ProductName = "Levis Exterior Paint White 2.5L",
                        Description = "Exterior Paint White 2.5L by Levis.",
                        Barcode = 500000005,
                        Stock = 62,
                        Price = 36.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = levisBrand,
                        ShelfId = flexaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000006"),
                        ProductName = "Flexa Exterior Paint Grey 2.5L",
                        Description = "Exterior Paint Grey 2.5L by Flexa.",
                        Barcode = 500000006,
                        Stock = 69,
                        Price = 37.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = flexaBrand,
                        ShelfId = flexaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000007"),
                        ProductName = "Sigma Coatings Primer 1L",
                        Description = "Primer 1L by Sigma Coatings.",
                        Barcode = 500000007,
                        Stock = 76,
                        Price = 16.49m,
                        Location = 2002,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sigmaBrand,
                        ShelfId = flexaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000008"),
                        ProductName = "Histor Lacquer White 0.75L",
                        Description = "Lacquer White 0.75L by Histor.",
                        Barcode = 500000008,
                        Stock = 83,
                        Price = 34.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = historBrand,
                        ShelfId = flexaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000009"),
                        ProductName = "Sikkens Wood Stain Oak 0.75L",
                        Description = "Wood Stain Oak 0.75L by Sikkens.",
                        Barcode = 500000009,
                        Stock = 90,
                        Price = 35.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sikkensBrand,
                        ShelfId = flexaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000000a"),
                        ProductName = "Levis Ceiling Paint White 5L",
                        Description = "Ceiling Paint White 5L by Levis.",
                        Barcode = 500000010,
                        Stock = 97,
                        Price = 36.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = levisBrand,
                        ShelfId = flexaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000000b"),
                        ProductName = "Flexa S2U Wall Paint White",
                        Description = "S2U Wall Paint White by Flexa.",
                        Barcode = 500000011,
                        Stock = 104,
                        Price = 20.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = flexaBrand,
                        ShelfId = sigmaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000000c"),
                        ProductName = "Sigma Coatings Latex Grey",
                        Description = "Latex Grey by Sigma Coatings.",
                        Barcode = 500000012,
                        Stock = 111,
                        Price = 21.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sigmaBrand,
                        ShelfId = sigmaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000000d"),
                        ProductName = "Histor Latex Beige",
                        Description = "Latex Beige by Histor.",
                        Barcode = 500000013,
                        Stock = 118,
                        Price = 17.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = historBrand,
                        ShelfId = sigmaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000000e"),
                        ProductName = "Sikkens Wall Paint Satin White",
                        Description = "Wall Paint Satin White by Sikkens.",
                        Barcode = 500000014,
                        Stock = 125,
                        Price = 18.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sikkensBrand,
                        ShelfId = sigmaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000000f"),
                        ProductName = "Levis Exterior Wall Paint White",
                        Description = "Exterior Wall Paint White by Levis.",
                        Barcode = 500000015,
                        Stock = 132,
                        Price = 19.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = levisBrand,
                        ShelfId = sigmaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000010"),
                        ProductName = "Flexa Wood Paint White 0.75L",
                        Description = "Wood Paint White 0.75L by Flexa.",
                        Barcode = 500000016,
                        Stock = 139,
                        Price = 37.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = flexaBrand,
                        ShelfId = sigmaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000011"),
                        ProductName = "Sigma Coatings Primer Universal 1L",
                        Description = "Primer Universal 1L by Sigma Coatings.",
                        Barcode = 500000017,
                        Stock = 146,
                        Price = 16.49m,
                        Location = 2002,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sigmaBrand,
                        ShelfId = sigmaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000012"),
                        ProductName = "Histor Wall Paint Blue 1L",
                        Description = "Wall Paint Blue 1L by Histor.",
                        Barcode = 500000018,
                        Stock = 23,
                        Price = 12.49m,
                        Location = 2003,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = historBrand,
                        ShelfId = sigmaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000013"),
                        ProductName = "Sikkens Wall Paint Green 1L",
                        Description = "Wall Paint Green 1L by Sikkens.",
                        Barcode = 500000019,
                        Stock = 30,
                        Price = 13.49m,
                        Location = 2004,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sikkensBrand,
                        ShelfId = sigmaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000014"),
                        ProductName = "Levis Ceiling Paint 5L",
                        Description = "Ceiling Paint 5L by Levis.",
                        Barcode = 500000020,
                        Stock = 37,
                        Price = 36.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = levisBrand,
                        ShelfId = sigmaShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000015"),
                        ProductName = "Flexa Wall Paint Cream",
                        Description = "Wall Paint Cream by Flexa.",
                        Barcode = 500000021,
                        Stock = 44,
                        Price = 20.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = flexaBrand,
                        ShelfId = historShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000016"),
                        ProductName = "Sigma Coatings Lime Paint White",
                        Description = "Lime Paint White by Sigma Coatings.",
                        Barcode = 500000022,
                        Stock = 51,
                        Price = 21.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sigmaBrand,
                        ShelfId = historShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000017"),
                        ProductName = "Histor Wall Paint Satin Grey",
                        Description = "Wall Paint Satin Grey by Histor.",
                        Barcode = 500000023,
                        Stock = 58,
                        Price = 17.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = historBrand,
                        ShelfId = historShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000018"),
                        ProductName = "Sikkens Latex White 5L",
                        Description = "Latex White 5L by Sikkens.",
                        Barcode = 500000024,
                        Stock = 65,
                        Price = 35.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sikkensBrand,
                        ShelfId = historShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000019"),
                        ProductName = "Levis Wood Lacquer White 0.75L",
                        Description = "Wood Lacquer White 0.75L by Levis.",
                        Barcode = 500000025,
                        Stock = 72,
                        Price = 36.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = levisBrand,
                        ShelfId = historShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000001a"),
                        ProductName = "Flexa Wood Primer 1L",
                        Description = "Wood Primer 1L by Flexa.",
                        Barcode = 500000026,
                        Stock = 79,
                        Price = 15.49m,
                        Location = 2001,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = flexaBrand,
                        ShelfId = historShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000001b"),
                        ProductName = "Sigma Coatings Wall Paint Terracotta",
                        Description = "Wall Paint Terracotta by Sigma Coatings.",
                        Barcode = 500000027,
                        Stock = 86,
                        Price = 21.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sigmaBrand,
                        ShelfId = historShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000001c"),
                        ProductName = "Histor Wall Paint Taupe",
                        Description = "Wall Paint Taupe by Histor.",
                        Barcode = 500000028,
                        Stock = 93,
                        Price = 17.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = historBrand,
                        ShelfId = historShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000001d"),
                        ProductName = "Sikkens Exterior Paint Grey 2.5L",
                        Description = "Exterior Paint Grey 2.5L by Sikkens.",
                        Barcode = 500000029,
                        Stock = 100,
                        Price = 35.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = sikkensBrand,
                        ShelfId = historShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000001e"),
                        ProductName = "Levis Ceiling Paint Matt 5L",
                        Description = "Ceiling Paint Matt 5L by Levis.",
                        Barcode = 500000030,
                        Stock = 107,
                        Price = 36.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = levisBrand,
                        ShelfId = historShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000001f"),
                        ProductName = "GAMMA Universal Screws 4x30mm",
                        Description = "Universal Screws 4x30mm by GAMMA.",
                        Barcode = 500000031,
                        Stock = 114,
                        Price = 6.49m,
                        Location = 1001,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = gammaBrand,
                        ShelfId = screwsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000020"),
                        ProductName = "Bosch Universal Screws 5x50mm",
                        Description = "Universal Screws 5x50mm by Bosch.",
                        Barcode = 500000032,
                        Stock = 121,
                        Price = 7.49m,
                        Location = 1002,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = boschBrand,
                        ShelfId = screwsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000021"),
                        ProductName = "Makita Chipboard Screws 3.5x20mm",
                        Description = "Chipboard Screws 3.5x20mm by Makita.",
                        Barcode = 500000033,
                        Stock = 128,
                        Price = 29.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = makitaBrand,
                        ShelfId = screwsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000022"),
                        ProductName = "Fischer Torx Screws 4x40mm",
                        Description = "Torx Screws 4x40mm by Fischer.",
                        Barcode = 500000034,
                        Stock = 135,
                        Price = 3.49m,
                        Location = 1004,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = fischerBrand,
                        ShelfId = screwsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000023"),
                        ProductName = "Facom Stainless Steel Screws 4x25mm",
                        Description = "Stainless Steel Screws 4x25mm by Facom.",
                        Barcode = 500000035,
                        Stock = 142,
                        Price = 3.49m,
                        Location = 1005,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = facomBrand,
                        ShelfId = screwsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000024"),
                        ProductName = "GAMMA Wood Screws 5x60mm",
                        Description = "Wood Screws 5x60mm by GAMMA.",
                        Barcode = 500000036,
                        Stock = 149,
                        Price = 15.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = gammaBrand,
                        ShelfId = screwsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000025"),
                        ProductName = "Bosch Drywall Screws 3.5x25mm",
                        Description = "Drywall Screws 3.5x25mm by Bosch.",
                        Barcode = 500000037,
                        Stock = 26,
                        Price = 16.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = boschBrand,
                        ShelfId = screwsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000026"),
                        ProductName = "Makita Concrete Screws 7.5x100mm",
                        Description = "Concrete Screws 7.5x100mm by Makita.",
                        Barcode = 500000038,
                        Stock = 33,
                        Price = 17.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = makitaBrand,
                        ShelfId = screwsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000027"),
                        ProductName = "Fischer Self-Drilling Screws 4.2x13mm",
                        Description = "Self-Drilling Screws 4.2x13mm by Fischer.",
                        Barcode = 500000039,
                        Stock = 40,
                        Price = 7.49m,
                        Location = 2004,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = fischerBrand,
                        ShelfId = screwsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000028"),
                        ProductName = "Facom Hex Head Screws 6x80mm",
                        Description = "Hex Head Screws 6x80mm by Facom.",
                        Barcode = 500000040,
                        Stock = 47,
                        Price = 19.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = facomBrand,
                        ShelfId = screwsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000029"),
                        ProductName = "GAMMA Nails 50mm",
                        Description = "Nails 50mm by GAMMA.",
                        Barcode = 500000041,
                        Stock = 54,
                        Price = 3.49m,
                        Location = 1001,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = gammaBrand,
                        ShelfId = nailsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000002a"),
                        ProductName = "Bosch Nails 80mm",
                        Description = "Nails 80mm by Bosch.",
                        Barcode = 500000042,
                        Stock = 61,
                        Price = 4.49m,
                        Location = 1002,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = boschBrand,
                        ShelfId = nailsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000002b"),
                        ProductName = "Makita Wire Nails 40mm",
                        Description = "Wire Nails 40mm by Makita.",
                        Barcode = 500000043,
                        Stock = 68,
                        Price = 22.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = makitaBrand,
                        ShelfId = nailsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000002c"),
                        ProductName = "Fischer Concrete Nails 60mm",
                        Description = "Concrete Nails 60mm by Fischer.",
                        Barcode = 500000044,
                        Stock = 75,
                        Price = 23.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = fischerBrand,
                        ShelfId = nailsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000002d"),
                        ProductName = "Facom Staples 10mm (box)",
                        Description = "Staples 10mm (box) by Facom.",
                        Barcode = 500000045,
                        Stock = 82,
                        Price = 7.49m,
                        Location = 1005,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = facomBrand,
                        ShelfId = nailsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000002e"),
                        ProductName = "GAMMA Ring Shank Nails 70mm",
                        Description = "Ring Shank Nails 70mm by GAMMA.",
                        Barcode = 500000046,
                        Stock = 89,
                        Price = 25.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = gammaBrand,
                        ShelfId = nailsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000002f"),
                        ProductName = "Bosch Roofing Nails 30mm",
                        Description = "Roofing Nails 30mm by Bosch.",
                        Barcode = 500000047,
                        Stock = 96,
                        Price = 26.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = boschBrand,
                        ShelfId = nailsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000030"),
                        ProductName = "Makita Tacker Nails 14mm",
                        Description = "Tacker Nails 14mm by Makita.",
                        Barcode = 500000048,
                        Stock = 103,
                        Price = 4.49m,
                        Location = 2003,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = makitaBrand,
                        ShelfId = nailsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000031"),
                        ProductName = "Fischer Flat Head Nails 25mm",
                        Description = "Flat Head Nails 25mm by Fischer.",
                        Barcode = 500000049,
                        Stock = 110,
                        Price = 28.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = fischerBrand,
                        ShelfId = nailsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000032"),
                        ProductName = "Facom Galvanised Nails 100mm",
                        Description = "Galvanised Nails 100mm by Facom.",
                        Barcode = 500000050,
                        Stock = 117,
                        Price = 6.49m,
                        Location = 2005,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = facomBrand,
                        ShelfId = nailsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000033"),
                        ProductName = "GAMMA Cylinder Lock SKG2",
                        Description = "Cylinder Lock SKG2 by GAMMA.",
                        Barcode = 500000051,
                        Stock = 124,
                        Price = 30.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = gammaBrand,
                        ShelfId = locksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000034"),
                        ProductName = "Bosch Padlock 40mm",
                        Description = "Padlock 40mm by Bosch.",
                        Barcode = 500000052,
                        Stock = 131,
                        Price = 31.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = boschBrand,
                        ShelfId = locksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000035"),
                        ProductName = "Makita Cylinder Lock SKG3",
                        Description = "Cylinder Lock SKG3 by Makita.",
                        Barcode = 500000053,
                        Stock = 138,
                        Price = 32.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = makitaBrand,
                        ShelfId = locksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000036"),
                        ProductName = "Fischer Cabinet Lock Universal",
                        Description = "Cabinet Lock Universal by Fischer.",
                        Barcode = 500000054,
                        Stock = 145,
                        Price = 33.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = fischerBrand,
                        ShelfId = locksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000037"),
                        ProductName = "Facom Bike Lock Cable 90cm",
                        Description = "Bike Lock Cable 90cm by Facom.",
                        Barcode = 500000055,
                        Stock = 22,
                        Price = 14.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = facomBrand,
                        ShelfId = locksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000038"),
                        ProductName = "GAMMA Door Lock Surface-Mounted",
                        Description = "Door Lock Surface-Mounted by GAMMA.",
                        Barcode = 500000056,
                        Stock = 29,
                        Price = 15.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = gammaBrand,
                        ShelfId = locksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000039"),
                        ProductName = "Bosch Combination Lock 4-Digit",
                        Description = "Combination Lock 4-Digit by Bosch.",
                        Barcode = 500000057,
                        Stock = 36,
                        Price = 16.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = boschBrand,
                        ShelfId = locksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000003a"),
                        ProductName = "Makita Hook Lock Black",
                        Description = "Hook Lock Black by Makita.",
                        Barcode = 500000058,
                        Stock = 43,
                        Price = 17.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = makitaBrand,
                        ShelfId = locksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000003b"),
                        ProductName = "Fischer Multi-Point Lock",
                        Description = "Multi-Point Lock by Fischer.",
                        Barcode = 500000059,
                        Stock = 50,
                        Price = 18.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = fischerBrand,
                        ShelfId = locksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000003c"),
                        ProductName = "Facom Security Door Fittings Set",
                        Description = "Security Door Fittings Set by Facom.",
                        Barcode = 500000060,
                        Stock = 57,
                        Price = 19.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = facomBrand,
                        ShelfId = locksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000003d"),
                        ProductName = "Legrand Socket White",
                        Description = "Socket White by Legrand.",
                        Barcode = 500000061,
                        Stock = 64,
                        Price = 8.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = legrandBrand,
                        ShelfId = socketsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000003e"),
                        ProductName = "Philips Socket Black",
                        Description = "Socket Black by Philips.",
                        Barcode = 500000062,
                        Stock = 71,
                        Price = 9.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = philipsBrand,
                        ShelfId = socketsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000003f"),
                        ProductName = "Schneider Electric Double Socket White",
                        Description = "Double Socket White by Schneider Electric.",
                        Barcode = 500000063,
                        Stock = 78,
                        Price = 10.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = schneiderBrand,
                        ShelfId = socketsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000040"),
                        ProductName = "Niko Waterproof Socket IP44",
                        Description = "Waterproof Socket IP44 by Niko.",
                        Barcode = 500000064,
                        Stock = 85,
                        Price = 11.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = nikoBrand,
                        ShelfId = socketsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000041"),
                        ProductName = "ABB USB Socket White",
                        Description = "USB Socket White by ABB.",
                        Barcode = 500000065,
                        Stock = 92,
                        Price = 12.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = abbBrand,
                        ShelfId = socketsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000042"),
                        ProductName = "Legrand Flush-Mounted Socket Round",
                        Description = "Flush-Mounted Socket Round by Legrand.",
                        Barcode = 500000066,
                        Stock = 99,
                        Price = 13.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = legrandBrand,
                        ShelfId = socketsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000043"),
                        ProductName = "Philips Surface-Mounted Socket Grey",
                        Description = "Surface-Mounted Socket Grey by Philips.",
                        Barcode = 500000067,
                        Stock = 106,
                        Price = 6.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = philipsBrand,
                        ShelfId = socketsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000044"),
                        ProductName = "Schneider Electric Socket with Switch",
                        Description = "Socket with Switch by Schneider Electric.",
                        Barcode = 500000068,
                        Stock = 113,
                        Price = 7.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = schneiderBrand,
                        ShelfId = socketsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000045"),
                        ProductName = "Niko Floor Socket Box",
                        Description = "Floor Socket Box by Niko.",
                        Barcode = 500000069,
                        Stock = 120,
                        Price = 8.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = nikoBrand,
                        ShelfId = socketsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000046"),
                        ProductName = "ABB Socket Aluminium",
                        Description = "Socket Aluminium by ABB.",
                        Barcode = 500000070,
                        Stock = 127,
                        Price = 9.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = abbBrand,
                        ShelfId = socketsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000047"),
                        ProductName = "Legrand Plug 3-Pin",
                        Description = "Plug 3-Pin by Legrand.",
                        Barcode = 500000071,
                        Stock = 134,
                        Price = 4.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = legrandBrand,
                        ShelfId = plugsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000048"),
                        ProductName = "Philips Adapter Plug",
                        Description = "Adapter Plug by Philips.",
                        Barcode = 500000072,
                        Stock = 141,
                        Price = 11.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = philipsBrand,
                        ShelfId = plugsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000049"),
                        ProductName = "Schneider Electric Angled Plug White",
                        Description = "Angled Plug White by Schneider Electric.",
                        Barcode = 500000073,
                        Stock = 148,
                        Price = 6.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = schneiderBrand,
                        ShelfId = plugsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000004a"),
                        ProductName = "Niko Power Strip 4-Way",
                        Description = "Power Strip 4-Way by Niko.",
                        Barcode = 500000074,
                        Stock = 25,
                        Price = 21.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = nikoBrand,
                        ShelfId = plugsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000004b"),
                        ProductName = "ABB Power Strip 6-Way with Switch",
                        Description = "Power Strip 6-Way with Switch by ABB.",
                        Barcode = 500000075,
                        Stock = 32,
                        Price = 12.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = abbBrand,
                        ShelfId = plugsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000004c"),
                        ProductName = "Legrand Rubber Coupler Socket",
                        Description = "Rubber Coupler Socket by Legrand.",
                        Barcode = 500000076,
                        Stock = 39,
                        Price = 7.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = legrandBrand,
                        ShelfId = plugsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000004d"),
                        ProductName = "Philips Travel Adapter Universal",
                        Description = "Travel Adapter Universal by Philips.",
                        Barcode = 500000077,
                        Stock = 46,
                        Price = 8.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = philipsBrand,
                        ShelfId = plugsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000004e"),
                        ProductName = "Schneider Electric Cable Plug Black",
                        Description = "Cable Plug Black by Schneider Electric.",
                        Barcode = 500000078,
                        Stock = 53,
                        Price = 36.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = schneiderBrand,
                        ShelfId = plugsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000004f"),
                        ProductName = "Niko IP44 Outdoor Plug",
                        Description = "IP44 Outdoor Plug by Niko.",
                        Barcode = 500000079,
                        Stock = 60,
                        Price = 6.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = nikoBrand,
                        ShelfId = plugsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000050"),
                        ProductName = "ABB Timer Plug Digital",
                        Description = "Timer Plug Digital by ABB.",
                        Barcode = 500000080,
                        Stock = 67,
                        Price = 7.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = abbBrand,
                        ShelfId = plugsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000051"),
                        ProductName = "Legrand VOB Wire 2.5mm 100m",
                        Description = "VOB Wire 2.5mm 100m by Legrand.",
                        Barcode = 500000081,
                        Stock = 74,
                        Price = 39.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = legrandBrand,
                        ShelfId = wiresShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000052"),
                        ProductName = "Philips YMVK Cable 3x2.5mm 50m",
                        Description = "YMVK Cable 3x2.5mm 50m by Philips.",
                        Barcode = 500000082,
                        Stock = 81,
                        Price = 40.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = philipsBrand,
                        ShelfId = wiresShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000053"),
                        ProductName = "Schneider Electric VOB Wire 1.5mm 100m",
                        Description = "VOB Wire 1.5mm 100m by Schneider Electric.",
                        Barcode = 500000083,
                        Stock = 88,
                        Price = 41.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = schneiderBrand,
                        ShelfId = wiresShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000054"),
                        ProductName = "Niko YMVK Cable 5x2.5mm 25m",
                        Description = "YMVK Cable 5x2.5mm 25m by Niko.",
                        Barcode = 500000084,
                        Stock = 95,
                        Price = 42.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = nikoBrand,
                        ShelfId = wiresShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000055"),
                        ProductName = "ABB Speaker Cable 2x1.5mm 20m",
                        Description = "Speaker Cable 2x1.5mm 20m by ABB.",
                        Barcode = 500000085,
                        Stock = 102,
                        Price = 18.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = abbBrand,
                        ShelfId = wiresShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000056"),
                        ProductName = "Legrand Telephone Cable 4-Core 50m",
                        Description = "Telephone Cable 4-Core 50m by Legrand.",
                        Barcode = 500000086,
                        Stock = 109,
                        Price = 19.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = legrandBrand,
                        ShelfId = wiresShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000057"),
                        ProductName = "Philips Network Cable Cat6 50m",
                        Description = "Network Cable Cat6 50m by Philips.",
                        Barcode = 500000087,
                        Stock = 116,
                        Price = 20.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = philipsBrand,
                        ShelfId = wiresShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000058"),
                        ProductName = "Schneider Electric Coax Cable 25m",
                        Description = "Coax Cable 25m by Schneider Electric.",
                        Barcode = 500000088,
                        Stock = 123,
                        Price = 21.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = schneiderBrand,
                        ShelfId = wiresShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000059"),
                        ProductName = "Niko Extension Cord 10m",
                        Description = "Extension Cord 10m by Niko.",
                        Barcode = 500000089,
                        Stock = 130,
                        Price = 12.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = nikoBrand,
                        ShelfId = wiresShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000005a"),
                        ProductName = "ABB Underground Cable YMVK 3x4mm 25m",
                        Description = "Underground Cable YMVK 3x4mm 25m by ABB.",
                        Barcode = 500000090,
                        Stock = 137,
                        Price = 23.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = abbBrand,
                        ShelfId = wiresShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000005b"),
                        ProductName = "Praxis Spruce Plank 200x20cm",
                        Description = "Spruce Plank 200x20cm by Praxis.",
                        Barcode = 500000091,
                        Stock = 144,
                        Price = 22.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = praxisBrand,
                        ShelfId = planksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000005c"),
                        ProductName = "Kronoply Plywood Board 122x244cm",
                        Description = "Plywood Board 122x244cm by Kronoply.",
                        Barcode = 500000092,
                        Stock = 21,
                        Price = 23.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = kronoplyBrand,
                        ShelfId = planksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000005d"),
                        ProductName = "Egger OSB Board 122x244cm",
                        Description = "OSB Board 122x244cm by Egger.",
                        Barcode = 500000093,
                        Stock = 28,
                        Price = 30.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = eggerBrand,
                        ShelfId = planksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000005e"),
                        ProductName = "Steico Underlayment Board 122x244cm",
                        Description = "Underlayment Board 122x244cm by Steico.",
                        Barcode = 500000094,
                        Stock = 35,
                        Price = 31.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = steicoBrand,
                        ShelfId = planksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000005f"),
                        ProductName = "Timberpro Planed Plank 240x14.5cm",
                        Description = "Planed Plank 240x14.5cm by Timberpro.",
                        Barcode = 500000095,
                        Stock = 42,
                        Price = 11.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = timberproBrand,
                        ShelfId = planksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000060"),
                        ProductName = "Praxis Pine Plank 180x15cm",
                        Description = "Pine Plank 180x15cm by Praxis.",
                        Barcode = 500000096,
                        Stock = 49,
                        Price = 12.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = praxisBrand,
                        ShelfId = planksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000061"),
                        ProductName = "Kronoply Spruce Batten 240x4.4cm",
                        Description = "Spruce Batten 240x4.4cm by Kronoply.",
                        Barcode = 500000097,
                        Stock = 56,
                        Price = 13.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = kronoplyBrand,
                        ShelfId = planksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000062"),
                        ProductName = "Egger MDF Board 244x122cm",
                        Description = "MDF Board 244x122cm by Egger.",
                        Barcode = 500000098,
                        Stock = 63,
                        Price = 35.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = eggerBrand,
                        ShelfId = planksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000063"),
                        ProductName = "Steico Triplex Board 122x250cm",
                        Description = "Triplex Board 122x250cm by Steico.",
                        Barcode = 500000099,
                        Stock = 70,
                        Price = 15.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = steicoBrand,
                        ShelfId = planksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000064"),
                        ProductName = "Timberpro Scaffold Board 300x19.5cm",
                        Description = "Scaffold Board 300x19.5cm by Timberpro.",
                        Barcode = 500000100,
                        Stock = 77,
                        Price = 16.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = timberproBrand,
                        ShelfId = planksShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000065"),
                        ProductName = "Praxis Beam 7x7x300cm",
                        Description = "Beam 7x7x300cm by Praxis.",
                        Barcode = 500000101,
                        Stock = 84,
                        Price = 30.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = praxisBrand,
                        ShelfId = beamsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000066"),
                        ProductName = "Kronoply Beam 10x10x400cm",
                        Description = "Beam 10x10x400cm by Kronoply.",
                        Barcode = 500000102,
                        Stock = 91,
                        Price = 31.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = kronoplyBrand,
                        ShelfId = beamsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000067"),
                        ProductName = "Egger Spruce Beam 6x8x300cm",
                        Description = "Spruce Beam 6x8x300cm by Egger.",
                        Barcode = 500000103,
                        Stock = 98,
                        Price = 32.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = eggerBrand,
                        ShelfId = beamsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000068"),
                        ProductName = "Steico Batten 4.5x7x300cm",
                        Description = "Batten 4.5x7x300cm by Steico.",
                        Barcode = 500000104,
                        Stock = 105,
                        Price = 33.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = steicoBrand,
                        ShelfId = beamsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000069"),
                        ProductName = "Timberpro Construction Beam 12x12x400cm",
                        Description = "Construction Beam 12x12x400cm by Timberpro.",
                        Barcode = 500000105,
                        Stock = 112,
                        Price = 34.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = timberproBrand,
                        ShelfId = beamsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000006a"),
                        ProductName = "Praxis Roof Beam 5x15x400cm",
                        Description = "Roof Beam 5x15x400cm by Praxis.",
                        Barcode = 500000106,
                        Stock = 119,
                        Price = 35.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = praxisBrand,
                        ShelfId = beamsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000006b"),
                        ProductName = "Kronoply Spruce Batten 3.8x8.8x300cm",
                        Description = "Spruce Batten 3.8x8.8x300cm by Kronoply.",
                        Barcode = 500000107,
                        Stock = 126,
                        Price = 36.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = kronoplyBrand,
                        ShelfId = beamsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000006c"),
                        ProductName = "Egger Planed Beam 9x9x300cm",
                        Description = "Planed Beam 9x9x300cm by Egger.",
                        Barcode = 500000108,
                        Stock = 133,
                        Price = 37.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = eggerBrand,
                        ShelfId = beamsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000006d"),
                        ProductName = "Steico Edge Board Beam 2x10x300cm",
                        Description = "Edge Board Beam 2x10x300cm by Steico.",
                        Barcode = 500000109,
                        Stock = 140,
                        Price = 38.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = steicoBrand,
                        ShelfId = beamsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000006e"),
                        ProductName = "Timberpro Floor Joist 5x20x400cm",
                        Description = "Floor Joist 5x20x400cm by Timberpro.",
                        Barcode = 500000110,
                        Stock = 147,
                        Price = 39.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = timberproBrand,
                        ShelfId = beamsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000006f"),
                        ProductName = "Praxis Impregnated Plank 200x2x20cm",
                        Description = "Impregnated Plank 200x2x20cm by Praxis.",
                        Barcode = 500000111,
                        Stock = 24,
                        Price = 12.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = praxisBrand,
                        ShelfId = impregnatedWoodShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000070"),
                        ProductName = "Kronoply Impregnated Post 7x7x250cm",
                        Description = "Impregnated Post 7x7x250cm by Kronoply.",
                        Barcode = 500000112,
                        Stock = 31,
                        Price = 36.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = kronoplyBrand,
                        ShelfId = impregnatedWoodShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000071"),
                        ProductName = "Egger Impregnated Post 9x9x300cm",
                        Description = "Impregnated Post 9x9x300cm by Egger.",
                        Barcode = 500000113,
                        Stock = 38,
                        Price = 37.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = eggerBrand,
                        ShelfId = impregnatedWoodShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000072"),
                        ProductName = "Steico Garden Fence Board 180x1.6x14cm",
                        Description = "Garden Fence Board 180x1.6x14cm by Steico.",
                        Barcode = 500000114,
                        Stock = 45,
                        Price = 15.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = steicoBrand,
                        ShelfId = impregnatedWoodShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000073"),
                        ProductName = "Timberpro Impregnated Beam 6x8x300cm",
                        Description = "Impregnated Beam 6x8x300cm by Timberpro.",
                        Barcode = 500000115,
                        Stock = 52,
                        Price = 44.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = timberproBrand,
                        ShelfId = impregnatedWoodShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000074"),
                        ProductName = "Praxis Decking Board Impregnated 300x2.8x14.5cm",
                        Description = "Decking Board Impregnated 300x2.8x14.5cm by Praxis.",
                        Barcode = 500000116,
                        Stock = 59,
                        Price = 17.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = praxisBrand,
                        ShelfId = impregnatedWoodShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000075"),
                        ProductName = "Kronoply Pine Post Impregnated 7x7x200cm",
                        Description = "Pine Post Impregnated 7x7x200cm by Kronoply.",
                        Barcode = 500000117,
                        Stock = 66,
                        Price = 21.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = kronoplyBrand,
                        ShelfId = impregnatedWoodShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000076"),
                        ProductName = "Egger Impregnated Weatherboard 400x1.8x14.5cm",
                        Description = "Impregnated Weatherboard 400x1.8x14.5cm by Egger.",
                        Barcode = 500000118,
                        Stock = 73,
                        Price = 19.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = eggerBrand,
                        ShelfId = impregnatedWoodShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000077"),
                        ProductName = "Steico Fence Post Impregnated 8x8x270cm",
                        Description = "Fence Post Impregnated 8x8x270cm by Steico.",
                        Barcode = 500000119,
                        Stock = 80,
                        Price = 23.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = steicoBrand,
                        ShelfId = impregnatedWoodShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000078"),
                        ProductName = "Timberpro Sleeper Impregnated 200x10x10cm",
                        Description = "Sleeper Impregnated 200x10x10cm by Timberpro.",
                        Barcode = 500000120,
                        Stock = 87,
                        Price = 24.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = timberproBrand,
                        ShelfId = impregnatedWoodShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000079"),
                        ProductName = "Hansgrohe Rain Shower 25cm",
                        Description = "Rain Shower 25cm by Hansgrohe.",
                        Barcode = 500000121,
                        Stock = 94,
                        Price = 135.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = hansgroheBrand,
                        ShelfId = showersShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000007a"),
                        ProductName = "Villeroy & Boch Hand Shower Set",
                        Description = "Hand Shower Set by Villeroy & Boch.",
                        Barcode = 500000122,
                        Stock = 101,
                        Price = 136.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = villeroyBrand,
                        ShelfId = showersShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000007b"),
                        ProductName = "Grohe Shower Head Adjustable",
                        Description = "Shower Head Adjustable by Grohe.",
                        Barcode = 500000123,
                        Stock = 108,
                        Price = 137.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = groheBrand,
                        ShelfId = showersShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000007c"),
                        ProductName = "Duravit Glass Shower Wall 80cm",
                        Description = "Glass Shower Wall 80cm by Duravit.",
                        Barcode = 500000124,
                        Stock = 115,
                        Price = 138.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = duravitBrand,
                        ShelfId = showersShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000007d"),
                        ProductName = "Geberit Shower Drain Stainless Steel 90cm",
                        Description = "Shower Drain Stainless Steel 90cm by Geberit.",
                        Barcode = 500000125,
                        Stock = 122,
                        Price = 139.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = geberitBrand,
                        ShelfId = showersShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000007e"),
                        ProductName = "Hansgrohe Thermostatic Shower Tap",
                        Description = "Thermostatic Shower Tap by Hansgrohe.",
                        Barcode = 500000126,
                        Stock = 129,
                        Price = 140.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = hansgroheBrand,
                        ShelfId = showersShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000007f"),
                        ProductName = "Villeroy & Boch Shower System with Slide Rail",
                        Description = "Shower System with Slide Rail by Villeroy & Boch.",
                        Barcode = 500000127,
                        Stock = 136,
                        Price = 141.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = villeroyBrand,
                        ShelfId = showersShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000080"),
                        ProductName = "Grohe Walk-In Shower Set Chrome",
                        Description = "Walk-In Shower Set Chrome by Grohe.",
                        Barcode = 500000128,
                        Stock = 143,
                        Price = 142.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = groheBrand,
                        ShelfId = showersShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000081"),
                        ProductName = "Duravit Shower Cabin Door 90cm",
                        Description = "Shower Cabin Door 90cm by Duravit.",
                        Barcode = 500000129,
                        Stock = 20,
                        Price = 143.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = duravitBrand,
                        ShelfId = showersShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000082"),
                        ProductName = "Geberit Rain Shower 30cm Black",
                        Description = "Rain Shower 30cm Black by Geberit.",
                        Barcode = 500000130,
                        Stock = 27,
                        Price = 144.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = geberitBrand,
                        ShelfId = showersShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000083"),
                        ProductName = "Hansgrohe Kitchen Tap Chrome",
                        Description = "Kitchen Tap Chrome by Hansgrohe.",
                        Barcode = 500000131,
                        Stock = 34,
                        Price = 55.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = hansgroheBrand,
                        ShelfId = waterTapShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000084"),
                        ProductName = "Villeroy & Boch Bath Tap",
                        Description = "Bath Tap by Villeroy & Boch.",
                        Barcode = 500000132,
                        Stock = 41,
                        Price = 39.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = villeroyBrand,
                        ShelfId = waterTapShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000085"),
                        ProductName = "Grohe Basin Tap Chrome",
                        Description = "Basin Tap Chrome by Grohe.",
                        Barcode = 500000133,
                        Stock = 48,
                        Price = 40.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = groheBrand,
                        ShelfId = waterTapShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000086"),
                        ProductName = "Duravit Single-Lever Kitchen Mixer Tap",
                        Description = "Single-Lever Kitchen Mixer Tap by Duravit.",
                        Barcode = 500000134,
                        Stock = 55,
                        Price = 41.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = duravitBrand,
                        ShelfId = waterTapShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000087"),
                        ProductName = "Geberit Outdoor Patio Tap",
                        Description = "Outdoor Patio Tap by Geberit.",
                        Barcode = 500000135,
                        Stock = 62,
                        Price = 42.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = geberitBrand,
                        ShelfId = waterTapShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000088"),
                        ProductName = "Hansgrohe Boiling Water Tap",
                        Description = "Boiling Water Tap by Hansgrohe.",
                        Barcode = 500000136,
                        Stock = 69,
                        Price = 43.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = hansgroheBrand,
                        ShelfId = waterTapShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000089"),
                        ProductName = "Villeroy & Boch Basin Mixer Tap Black",
                        Description = "Basin Mixer Tap Black by Villeroy & Boch.",
                        Barcode = 500000137,
                        Stock = 76,
                        Price = 44.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = villeroyBrand,
                        ShelfId = waterTapShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000008a"),
                        ProductName = "Grohe Kitchen Mixer Tap Pull-Out",
                        Description = "Kitchen Mixer Tap Pull-Out by Grohe.",
                        Barcode = 500000138,
                        Stock = 83,
                        Price = 45.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = groheBrand,
                        ShelfId = waterTapShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000008b"),
                        ProductName = "Duravit Garden Tap with Hose Connector",
                        Description = "Garden Tap with Hose Connector by Duravit.",
                        Barcode = 500000139,
                        Stock = 90,
                        Price = 46.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = duravitBrand,
                        ShelfId = waterTapShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000008c"),
                        ProductName = "Geberit Thermostatic Bath Tap",
                        Description = "Thermostatic Bath Tap by Geberit.",
                        Barcode = 500000140,
                        Stock = 97,
                        Price = 47.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = geberitBrand,
                        ShelfId = waterTapShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000008d"),
                        ProductName = "Hansgrohe Anti-Slip Shower Mat White",
                        Description = "Anti-Slip Shower Mat White by Hansgrohe.",
                        Barcode = 500000141,
                        Stock = 104,
                        Price = 18.99m,
                        Location = 1001,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = hansgroheBrand,
                        ShelfId = showerMatsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000008e"),
                        ProductName = "Villeroy & Boch Anti-Slip Shower Mat Grey",
                        Description = "Anti-Slip Shower Mat Grey by Villeroy & Boch.",
                        Barcode = 500000142,
                        Stock = 111,
                        Price = 19.99m,
                        Location = 1002,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = villeroyBrand,
                        ShelfId = showerMatsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000008f"),
                        ProductName = "Grohe Anti-Slip Shower Mat Black",
                        Description = "Anti-Slip Shower Mat Black by Grohe.",
                        Barcode = 500000143,
                        Stock = 118,
                        Price = 20.99m,
                        Location = 1003,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = groheBrand,
                        ShelfId = showerMatsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000090"),
                        ProductName = "Duravit Bamboo Shower Mat",
                        Description = "Bamboo Shower Mat by Duravit.",
                        Barcode = 500000144,
                        Stock = 125,
                        Price = 21.99m,
                        Location = 1004,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = duravitBrand,
                        ShelfId = showerMatsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000091"),
                        ProductName = "Geberit Shower Mat Rubber Anti-Slip",
                        Description = "Shower Mat Rubber Anti-Slip by Geberit.",
                        Barcode = 500000145,
                        Stock = 132,
                        Price = 22.99m,
                        Location = 1005,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = geberitBrand,
                        ShelfId = showerMatsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000092"),
                        ProductName = "Hansgrohe Shower Mat XL White",
                        Description = "Shower Mat XL White by Hansgrohe.",
                        Barcode = 500000146,
                        Stock = 139,
                        Price = 23.99m,
                        Location = 2001,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = hansgroheBrand,
                        ShelfId = showerMatsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000093"),
                        ProductName = "Villeroy & Boch Shower Mat with Suction Cups",
                        Description = "Shower Mat with Suction Cups by Villeroy & Boch.",
                        Barcode = 500000147,
                        Stock = 146,
                        Price = 9.99m,
                        Location = 2002,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = villeroyBrand,
                        ShelfId = showerMatsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000094"),
                        ProductName = "Grohe Shower Mat Plastic Grey",
                        Description = "Shower Mat Plastic Grey by Grohe.",
                        Barcode = 500000148,
                        Stock = 23,
                        Price = 10.99m,
                        Location = 2003,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = groheBrand,
                        ShelfId = showerMatsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000095"),
                        ProductName = "Duravit Anti-Slip Bath Mat Blue",
                        Description = "Anti-Slip Bath Mat Blue by Duravit.",
                        Barcode = 500000149,
                        Stock = 30,
                        Price = 11.99m,
                        Location = 2004,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = duravitBrand,
                        ShelfId = showerMatsShelf
                    },
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000096"),
                        ProductName = "Geberit Shower Mat Antibacterial White",
                        Description = "Shower Mat Antibacterial White by Geberit.",
                        Barcode = 500000150,
                        Stock = 37,
                        Price = 12.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = geberitBrand,
                        ShelfId = showerMatsShelf
                    },
                };

                context.Set<Product>().AddRange(Products);
                context.SaveChanges();
            }
        }
    }
}