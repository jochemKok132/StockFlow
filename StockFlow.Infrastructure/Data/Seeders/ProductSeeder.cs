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

                // Shelf ids (from ShelfSeeder)
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
                    // Flexa shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000001"),
                        ProductName = "Flexa Verf Muurverf Wit 1L",
                        Description = "Muurverf Wit 1L van Flexa Verf.",
                        Barcode = 500001,
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
                        ProductName = "Sigma Coatings Muurverf Grijs 1L",
                        Description = "Muurverf Grijs 1L van Sigma Coatings.",
                        Barcode = 500002,
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
                        ProductName = "Histor Muurverf Zwart 1L",
                        Description = "Muurverf Zwart 1L van Histor.",
                        Barcode = 500003,
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
                        ProductName = "Sikkens Muurverf Beige 1L",
                        Description = "Muurverf Beige 1L van Sikkens.",
                        Barcode = 500004,
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
                        ProductName = "Levis Verf Buitenverf Wit 2.5L",
                        Description = "Buitenverf Wit 2.5L van Levis Verf.",
                        Barcode = 500005,
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
                        ProductName = "Flexa Verf Buitenverf Grijs 2.5L",
                        Description = "Buitenverf Grijs 2.5L van Flexa Verf.",
                        Barcode = 500006,
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
                        ProductName = "Sigma Coatings Grondverf 1L",
                        Description = "Grondverf 1L van Sigma Coatings.",
                        Barcode = 500007,
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
                        ProductName = "Histor Lak Wit 0.75L",
                        Description = "Lak Wit 0.75L van Histor.",
                        Barcode = 500008,
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
                        ProductName = "Sikkens Houtbeits Eiken 0.75L",
                        Description = "Houtbeits Eiken 0.75L van Sikkens.",
                        Barcode = 500009,
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
                        ProductName = "Levis Verf Plafondverf Wit 5L",
                        Description = "Plafondverf Wit 5L van Levis Verf.",
                        Barcode = 500010,
                        Stock = 97,
                        Price = 36.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = levisBrand,
                        ShelfId = flexaShelf
                    },
                    // Sigma shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000000b"),
                        ProductName = "Flexa Verf S2U Muurverf Wit",
                        Description = "S2U Muurverf Wit van Flexa Verf.",
                        Barcode = 500011,
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
                        ProductName = "Sigma Coatings Latex Grijs",
                        Description = "Latex Grijs van Sigma Coatings.",
                        Barcode = 500012,
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
                        Description = "Latex Beige van Histor.",
                        Barcode = 500013,
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
                        ProductName = "Sikkens Muurverf Zijdeglans Wit",
                        Description = "Muurverf Zijdeglans Wit van Sikkens.",
                        Barcode = 500014,
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
                        ProductName = "Levis Verf Buitenmuurverf Wit",
                        Description = "Buitenmuurverf Wit van Levis Verf.",
                        Barcode = 500015,
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
                        ProductName = "Flexa Verf Houtverf Wit 0.75L",
                        Description = "Houtverf Wit 0.75L van Flexa Verf.",
                        Barcode = 500016,
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
                        ProductName = "Sigma Coatings Grondverf Universeel 1L",
                        Description = "Grondverf Universeel 1L van Sigma Coatings.",
                        Barcode = 500017,
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
                        ProductName = "Histor Muurverf Blauw 1L",
                        Description = "Muurverf Blauw 1L van Histor.",
                        Barcode = 500018,
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
                        ProductName = "Sikkens Muurverf Groen 1L",
                        Description = "Muurverf Groen 1L van Sikkens.",
                        Barcode = 500019,
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
                        ProductName = "Levis Verf Plafondverf 5L",
                        Description = "Plafondverf 5L van Levis Verf.",
                        Barcode = 500020,
                        Stock = 37,
                        Price = 36.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = levisBrand,
                        ShelfId = sigmaShelf
                    },
                    // Histor shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000015"),
                        ProductName = "Flexa Verf Muurverf Creme",
                        Description = "Muurverf Creme van Flexa Verf.",
                        Barcode = 500021,
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
                        ProductName = "Sigma Coatings Kalkverf Wit",
                        Description = "Kalkverf Wit van Sigma Coatings.",
                        Barcode = 500022,
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
                        ProductName = "Histor Muurverf Zijdeglans Grijs",
                        Description = "Muurverf Zijdeglans Grijs van Histor.",
                        Barcode = 500023,
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
                        ProductName = "Sikkens Latex Wit 5L",
                        Description = "Latex Wit 5L van Sikkens.",
                        Barcode = 500024,
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
                        ProductName = "Levis Verf Houtlak Wit 0.75L",
                        Description = "Houtlak Wit 0.75L van Levis Verf.",
                        Barcode = 500025,
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
                        ProductName = "Flexa Verf Grondverf Hout 1L",
                        Description = "Grondverf Hout 1L van Flexa Verf.",
                        Barcode = 500026,
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
                        ProductName = "Sigma Coatings Muurverf Terracotta",
                        Description = "Muurverf Terracotta van Sigma Coatings.",
                        Barcode = 500027,
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
                        ProductName = "Histor Muurverf Taupe",
                        Description = "Muurverf Taupe van Histor.",
                        Barcode = 500028,
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
                        ProductName = "Sikkens Buitenverf Grijs 2.5L",
                        Description = "Buitenverf Grijs 2.5L van Sikkens.",
                        Barcode = 500029,
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
                        ProductName = "Levis Verf Plafondverf Mat 5L",
                        Description = "Plafondverf Mat 5L van Levis Verf.",
                        Barcode = 500030,
                        Stock = 107,
                        Price = 36.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "paint" },
                        BrandId = levisBrand,
                        ShelfId = historShelf
                    },
                    // Screws shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000001f"),
                        ProductName = "GAMMA Universele Schroeven 4x30mm",
                        Description = "Universele Schroeven 4x30mm van GAMMA.",
                        Barcode = 500031,
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
                        ProductName = "Bosch Universele Schroeven 5x50mm",
                        Description = "Universele Schroeven 5x50mm van Bosch.",
                        Barcode = 500032,
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
                        ProductName = "Makita Spaanplaatschroeven 3.5x20mm",
                        Description = "Spaanplaatschroeven 3.5x20mm van Makita.",
                        Barcode = 500033,
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
                        ProductName = "Fischer Torx Schroeven 4x40mm",
                        Description = "Torx Schroeven 4x40mm van Fischer.",
                        Barcode = 500034,
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
                        ProductName = "Facom RVS Schroeven 4x25mm",
                        Description = "RVS Schroeven 4x25mm van Facom.",
                        Barcode = 500035,
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
                        ProductName = "GAMMA Houtschroeven 5x60mm",
                        Description = "Houtschroeven 5x60mm van GAMMA.",
                        Barcode = 500036,
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
                        ProductName = "Bosch Gipsplaatschroeven 3.5x25mm",
                        Description = "Gipsplaatschroeven 3.5x25mm van Bosch.",
                        Barcode = 500037,
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
                        ProductName = "Makita Betonschroeven 7.5x100mm",
                        Description = "Betonschroeven 7.5x100mm van Makita.",
                        Barcode = 500038,
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
                        ProductName = "Fischer Zelfborende Schroeven 4.2x13mm",
                        Description = "Zelfborende Schroeven 4.2x13mm van Fischer.",
                        Barcode = 500039,
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
                        ProductName = "Facom Kolomschroeven 6x80mm",
                        Description = "Kolomschroeven 6x80mm van Facom.",
                        Barcode = 500040,
                        Stock = 47,
                        Price = 19.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = facomBrand,
                        ShelfId = screwsShelf
                    },
                    // Nails shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000029"),
                        ProductName = "GAMMA Spijkers 50mm",
                        Description = "Spijkers 50mm van GAMMA.",
                        Barcode = 500041,
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
                        ProductName = "Bosch Spijkers 80mm",
                        Description = "Spijkers 80mm van Bosch.",
                        Barcode = 500042,
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
                        ProductName = "Makita Draadnagels 40mm",
                        Description = "Draadnagels 40mm van Makita.",
                        Barcode = 500043,
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
                        ProductName = "Fischer Betonnagels 60mm",
                        Description = "Betonnagels 60mm van Fischer.",
                        Barcode = 500044,
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
                        ProductName = "Facom Nietjes 10mm (doos)",
                        Description = "Nietjes 10mm (doos) van Facom.",
                        Barcode = 500045,
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
                        ProductName = "GAMMA Ringnagels 70mm",
                        Description = "Ringnagels 70mm van GAMMA.",
                        Barcode = 500046,
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
                        ProductName = "Bosch Dakspijkers 30mm",
                        Description = "Dakspijkers 30mm van Bosch.",
                        Barcode = 500047,
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
                        ProductName = "Makita Tacker Nagels 14mm",
                        Description = "Tacker Nagels 14mm van Makita.",
                        Barcode = 500048,
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
                        ProductName = "Fischer Kopspijkers 25mm",
                        Description = "Kopspijkers 25mm van Fischer.",
                        Barcode = 500049,
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
                        ProductName = "Facom Verzinkte Spijkers 100mm",
                        Description = "Verzinkte Spijkers 100mm van Facom.",
                        Barcode = 500050,
                        Stock = 117,
                        Price = 6.49m,
                        Location = 2005,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = facomBrand,
                        ShelfId = nailsShelf
                    },
                    // Locks shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000033"),
                        ProductName = "GAMMA Cilinderslot SKG2",
                        Description = "Cilinderslot SKG2 van GAMMA.",
                        Barcode = 500051,
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
                        ProductName = "Bosch Hangslot 40mm",
                        Description = "Hangslot 40mm van Bosch.",
                        Barcode = 500052,
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
                        ProductName = "Makita Cilinderslot SKG3",
                        Description = "Cilinderslot SKG3 van Makita.",
                        Barcode = 500053,
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
                        ProductName = "Fischer Kastslot Universeel",
                        Description = "Kastslot Universeel van Fischer.",
                        Barcode = 500054,
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
                        ProductName = "Facom Fietsslot Kabel 90cm",
                        Description = "Fietsslot Kabel 90cm van Facom.",
                        Barcode = 500055,
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
                        ProductName = "GAMMA Deurslot Opbouw",
                        Description = "Deurslot Opbouw van GAMMA.",
                        Barcode = 500056,
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
                        ProductName = "Bosch Combinatieslot 4-cijferig",
                        Description = "Combinatieslot 4-cijferig van Bosch.",
                        Barcode = 500057,
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
                        ProductName = "Makita Haakslot Zwart",
                        Description = "Haakslot Zwart van Makita.",
                        Barcode = 500058,
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
                        ProductName = "Fischer Meerpuntssluiting",
                        Description = "Meerpuntssluiting van Fischer.",
                        Barcode = 500059,
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
                        ProductName = "Facom Veiligheidsbeslag Set",
                        Description = "Veiligheidsbeslag Set van Facom.",
                        Barcode = 500060,
                        Stock = 57,
                        Price = 19.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "hardware" },
                        BrandId = facomBrand,
                        ShelfId = locksShelf
                    },
                    // Sockets shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000003d"),
                        ProductName = "Legrand Stopcontact Wit",
                        Description = "Stopcontact Wit van Legrand.",
                        Barcode = 500061,
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
                        ProductName = "Philips Stopcontact Zwart",
                        Description = "Stopcontact Zwart van Philips.",
                        Barcode = 500062,
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
                        ProductName = "Schneider Electric Dubbel Stopcontact Wit",
                        Description = "Dubbel Stopcontact Wit van Schneider Electric.",
                        Barcode = 500063,
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
                        ProductName = "Niko Waterdicht Stopcontact IP44",
                        Description = "Waterdicht Stopcontact IP44 van Niko.",
                        Barcode = 500064,
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
                        ProductName = "ABB USB Stopcontact Wit",
                        Description = "USB Stopcontact Wit van ABB.",
                        Barcode = 500065,
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
                        ProductName = "Legrand Inbouw Stopcontact Rond",
                        Description = "Inbouw Stopcontact Rond van Legrand.",
                        Barcode = 500066,
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
                        ProductName = "Philips Opbouw Stopcontact Grijs",
                        Description = "Opbouw Stopcontact Grijs van Philips.",
                        Barcode = 500067,
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
                        ProductName = "Schneider Electric Stopcontact met Schakelaar",
                        Description = "Stopcontact met Schakelaar van Schneider Electric.",
                        Barcode = 500068,
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
                        ProductName = "Niko Vloercontactdoos",
                        Description = "Vloercontactdoos van Niko.",
                        Barcode = 500069,
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
                        ProductName = "ABB Stopcontact Aluminium",
                        Description = "Stopcontact Aluminium van ABB.",
                        Barcode = 500070,
                        Stock = 127,
                        Price = 9.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = abbBrand,
                        ShelfId = socketsShelf
                    },
                    // Plugs shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000047"),
                        ProductName = "Legrand Stekker 3-polig",
                        Description = "Stekker 3-polig van Legrand.",
                        Barcode = 500071,
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
                        ProductName = "Philips Verloopstekker",
                        Description = "Verloopstekker van Philips.",
                        Barcode = 500072,
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
                        ProductName = "Schneider Electric Haakse Stekker Wit",
                        Description = "Haakse Stekker Wit van Schneider Electric.",
                        Barcode = 500073,
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
                        ProductName = "Niko Stekkerdoos 4-voudig",
                        Description = "Stekkerdoos 4-voudig van Niko.",
                        Barcode = 500074,
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
                        ProductName = "ABB Stekkerdoos 6-voudig met Schakelaar",
                        Description = "Stekkerdoos 6-voudig met Schakelaar van ABB.",
                        Barcode = 500075,
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
                        ProductName = "Legrand Contrastekker Rubber",
                        Description = "Contrastekker Rubber van Legrand.",
                        Barcode = 500076,
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
                        ProductName = "Philips Reisstekker Universeel",
                        Description = "Reisstekker Universeel van Philips.",
                        Barcode = 500077,
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
                        ProductName = "Schneider Electric Kabelstekker Zwart",
                        Description = "Kabelstekker Zwart van Schneider Electric.",
                        Barcode = 500078,
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
                        ProductName = "Niko IP44 Stekker Buiten",
                        Description = "IP44 Stekker Buiten van Niko.",
                        Barcode = 500079,
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
                        ProductName = "ABB Timer Stekker Digitaal",
                        Description = "Timer Stekker Digitaal van ABB.",
                        Barcode = 500080,
                        Stock = 67,
                        Price = 7.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = abbBrand,
                        ShelfId = plugsShelf
                    },
                    // Wires shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000051"),
                        ProductName = "Legrand VOB Draad 2.5mm 100m",
                        Description = "VOB Draad 2.5mm 100m van Legrand.",
                        Barcode = 500081,
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
                        ProductName = "Philips YMVK Kabel 3x2.5mm 50m",
                        Description = "YMVK Kabel 3x2.5mm 50m van Philips.",
                        Barcode = 500082,
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
                        ProductName = "Schneider Electric VOB Draad 1.5mm 100m",
                        Description = "VOB Draad 1.5mm 100m van Schneider Electric.",
                        Barcode = 500083,
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
                        ProductName = "Niko YMVK Kabel 5x2.5mm 25m",
                        Description = "YMVK Kabel 5x2.5mm 25m van Niko.",
                        Barcode = 500084,
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
                        ProductName = "ABB Luidsprekerkabel 2x1.5mm 20m",
                        Description = "Luidsprekerkabel 2x1.5mm 20m van ABB.",
                        Barcode = 500085,
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
                        ProductName = "Legrand Telefoonkabel 4-aderig 50m",
                        Description = "Telefoonkabel 4-aderig 50m van Legrand.",
                        Barcode = 500086,
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
                        ProductName = "Philips Netwerkkabel Cat6 50m",
                        Description = "Netwerkkabel Cat6 50m van Philips.",
                        Barcode = 500087,
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
                        ProductName = "Schneider Electric Coax Kabel 25m",
                        Description = "Coax Kabel 25m van Schneider Electric.",
                        Barcode = 500088,
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
                        ProductName = "Niko Verlengsnoer 10m",
                        Description = "Verlengsnoer 10m van Niko.",
                        Barcode = 500089,
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
                        ProductName = "ABB Grondkabel YMVK 3x4mm 25m",
                        Description = "Grondkabel YMVK 3x4mm 25m van ABB.",
                        Barcode = 500090,
                        Stock = 137,
                        Price = 23.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "electronics" },
                        BrandId = abbBrand,
                        ShelfId = wiresShelf
                    },
                    // Planks shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000005b"),
                        ProductName = "Praxis Bouwmaterialen Vurenhouten Plank 200x20cm",
                        Description = "Vurenhouten Plank 200x20cm van Praxis Bouwmaterialen.",
                        Barcode = 500091,
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
                        ProductName = "Kronoply Multiplex Plank 122x244cm",
                        Description = "Multiplex Plank 122x244cm van Kronoply.",
                        Barcode = 500092,
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
                        ProductName = "Egger OSB Plaat 122x244cm",
                        Description = "OSB Plaat 122x244cm van Egger.",
                        Barcode = 500093,
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
                        ProductName = "Steico Underlayment Plaat 122x244cm",
                        Description = "Underlayment Plaat 122x244cm van Steico.",
                        Barcode = 500094,
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
                        ProductName = "Timberpro Schaafwerk Plank 240x14.5cm",
                        Description = "Schaafwerk Plank 240x14.5cm van Timberpro.",
                        Barcode = 500095,
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
                        ProductName = "Praxis Bouwmaterialen Grenen Plank 180x15cm",
                        Description = "Grenen Plank 180x15cm van Praxis Bouwmaterialen.",
                        Barcode = 500096,
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
                        ProductName = "Kronoply Vurenhouten Lat 240x4.4cm",
                        Description = "Vurenhouten Lat 240x4.4cm van Kronoply.",
                        Barcode = 500097,
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
                        ProductName = "Egger MDF Plaat 244x122cm",
                        Description = "MDF Plaat 244x122cm van Egger.",
                        Barcode = 500098,
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
                        ProductName = "Steico Triplex Plank 122x250cm",
                        Description = "Triplex Plank 122x250cm van Steico.",
                        Barcode = 500099,
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
                        ProductName = "Timberpro Steigerplank 300x19.5cm",
                        Description = "Steigerplank 300x19.5cm van Timberpro.",
                        Barcode = 500100,
                        Stock = 77,
                        Price = 16.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = timberproBrand,
                        ShelfId = planksShelf
                    },
                    // Beams shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000065"),
                        ProductName = "Praxis Bouwmaterialen Balk 7x7x300cm",
                        Description = "Balk 7x7x300cm van Praxis Bouwmaterialen.",
                        Barcode = 500101,
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
                        ProductName = "Kronoply Balk 10x10x400cm",
                        Description = "Balk 10x10x400cm van Kronoply.",
                        Barcode = 500102,
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
                        ProductName = "Egger Vurenhouten Balk 6x8x300cm",
                        Description = "Vurenhouten Balk 6x8x300cm van Egger.",
                        Barcode = 500103,
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
                        ProductName = "Steico Regel 4.5x7x300cm",
                        Description = "Regel 4.5x7x300cm van Steico.",
                        Barcode = 500104,
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
                        ProductName = "Timberpro Constructiebalk 12x12x400cm",
                        Description = "Constructiebalk 12x12x400cm van Timberpro.",
                        Barcode = 500105,
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
                        ProductName = "Praxis Bouwmaterialen Dakbalk 5x15x400cm",
                        Description = "Dakbalk 5x15x400cm van Praxis Bouwmaterialen.",
                        Barcode = 500106,
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
                        ProductName = "Kronoply Vurenhouten Regel 3.8x8.8x300cm",
                        Description = "Vurenhouten Regel 3.8x8.8x300cm van Kronoply.",
                        Barcode = 500107,
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
                        ProductName = "Egger Balk Geschaafd 9x9x300cm",
                        Description = "Balk Geschaafd 9x9x300cm van Egger.",
                        Barcode = 500108,
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
                        ProductName = "Steico Kantplank Balk 2x10x300cm",
                        Description = "Kantplank Balk 2x10x300cm van Steico.",
                        Barcode = 500109,
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
                        ProductName = "Timberpro Vloerbalk 5x20x400cm",
                        Description = "Vloerbalk 5x20x400cm van Timberpro.",
                        Barcode = 500110,
                        Stock = 147,
                        Price = 39.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = timberproBrand,
                        ShelfId = beamsShelf
                    },
                    // Impregnated Wood shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000006f"),
                        ProductName = "Praxis Bouwmaterialen Geïmpregneerde Plank 200x2x20cm",
                        Description = "Geïmpregneerde Plank 200x2x20cm van Praxis Bouwmaterialen.",
                        Barcode = 500111,
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
                        ProductName = "Kronoply Geïmpregneerde Paal 7x7x250cm",
                        Description = "Geïmpregneerde Paal 7x7x250cm van Kronoply.",
                        Barcode = 500112,
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
                        ProductName = "Egger Geïmpregneerde Paal 9x9x300cm",
                        Description = "Geïmpregneerde Paal 9x9x300cm van Egger.",
                        Barcode = 500113,
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
                        ProductName = "Steico Tuinschutting Plank 180x1.6x14cm",
                        Description = "Tuinschutting Plank 180x1.6x14cm van Steico.",
                        Barcode = 500114,
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
                        ProductName = "Timberpro Geïmpregneerde Balk 6x8x300cm",
                        Description = "Geïmpregneerde Balk 6x8x300cm van Timberpro.",
                        Barcode = 500115,
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
                        ProductName = "Praxis Bouwmaterialen Vlonderplank Geïmpregneerd 300x2.8x14.5cm",
                        Description = "Vlonderplank Geïmpregneerd 300x2.8x14.5cm van Praxis Bouwmaterialen.",
                        Barcode = 500116,
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
                        ProductName = "Kronoply Grenen Paal Geïmpregneerd 7x7x200cm",
                        Description = "Grenen Paal Geïmpregneerd 7x7x200cm van Kronoply.",
                        Barcode = 500117,
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
                        ProductName = "Egger Geïmpregneerde Rabatplank 400x1.8x14.5cm",
                        Description = "Geïmpregneerde Rabatplank 400x1.8x14.5cm van Egger.",
                        Barcode = 500118,
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
                        ProductName = "Steico Schuttingpaal Geïmpregneerd 8x8x270cm",
                        Description = "Schuttingpaal Geïmpregneerd 8x8x270cm van Steico.",
                        Barcode = 500119,
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
                        ProductName = "Timberpro Biels Geïmpregneerd 200x10x10cm",
                        Description = "Biels Geïmpregneerd 200x10x10cm van Timberpro.",
                        Barcode = 500120,
                        Stock = 87,
                        Price = 24.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "wood" },
                        BrandId = timberproBrand,
                        ShelfId = impregnatedWoodShelf
                    },
                    // Showers shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000079"),
                        ProductName = "Hansgrohe Regendouche 25cm",
                        Description = "Regendouche 25cm van Hansgrohe.",
                        Barcode = 500121,
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
                        ProductName = "Villeroy & Boch Handdouche Set",
                        Description = "Handdouche Set van Villeroy & Boch.",
                        Barcode = 500122,
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
                        ProductName = "Grohe Douchekop Verstelbaar",
                        Description = "Douchekop Verstelbaar van Grohe.",
                        Barcode = 500123,
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
                        ProductName = "Duravit Glazen Douchewand 80cm",
                        Description = "Glazen Douchewand 80cm van Duravit.",
                        Barcode = 500124,
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
                        ProductName = "Geberit Douchegoot RVS 90cm",
                        Description = "Douchegoot RVS 90cm van Geberit.",
                        Barcode = 500125,
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
                        ProductName = "Hansgrohe Thermostaatkraan Douche",
                        Description = "Thermostaatkraan Douche van Hansgrohe.",
                        Barcode = 500126,
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
                        ProductName = "Villeroy & Boch Douchesysteem met Glijstang",
                        Description = "Douchesysteem met Glijstang van Villeroy & Boch.",
                        Barcode = 500127,
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
                        ProductName = "Grohe Inloopdouche Set Chroom",
                        Description = "Inloopdouche Set Chroom van Grohe.",
                        Barcode = 500128,
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
                        ProductName = "Duravit Douchecabine Deur 90cm",
                        Description = "Douchecabine Deur 90cm van Duravit.",
                        Barcode = 500129,
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
                        ProductName = "Geberit Regendouche 30cm Zwart",
                        Description = "Regendouche 30cm Zwart van Geberit.",
                        Barcode = 500130,
                        Stock = 27,
                        Price = 144.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = geberitBrand,
                        ShelfId = showersShelf
                    },
                    // Water Tap shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-000000000083"),
                        ProductName = "Hansgrohe Keukenkraan Chroom",
                        Description = "Keukenkraan Chroom van Hansgrohe.",
                        Barcode = 500131,
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
                        ProductName = "Villeroy & Boch Badkraan",
                        Description = "Badkraan van Villeroy & Boch.",
                        Barcode = 500132,
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
                        ProductName = "Grohe Wastafelkraan Chroom",
                        Description = "Wastafelkraan Chroom van Grohe.",
                        Barcode = 500133,
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
                        ProductName = "Duravit Eengreepsmengkraan Keuken",
                        Description = "Eengreepsmengkraan Keuken van Duravit.",
                        Barcode = 500134,
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
                        ProductName = "Geberit Terrasskraan Buiten",
                        Description = "Terrasskraan Buiten van Geberit.",
                        Barcode = 500135,
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
                        ProductName = "Hansgrohe Boilerkraan",
                        Description = "Boilerkraan van Hansgrohe.",
                        Barcode = 500136,
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
                        ProductName = "Villeroy & Boch Wastafelmengkraan Zwart",
                        Description = "Wastafelmengkraan Zwart van Villeroy & Boch.",
                        Barcode = 500137,
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
                        ProductName = "Grohe Keukenmengkraan Uittrekbaar",
                        Description = "Keukenmengkraan Uittrekbaar van Grohe.",
                        Barcode = 500138,
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
                        ProductName = "Duravit Tuinkraan met Slangaansluiting",
                        Description = "Tuinkraan met Slangaansluiting van Duravit.",
                        Barcode = 500139,
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
                        ProductName = "Geberit Thermostaatkraan Bad",
                        Description = "Thermostaatkraan Bad van Geberit.",
                        Barcode = 500140,
                        Stock = 97,
                        Price = 47.99m,
                        Location = 2005,
                        SalesTags = new List<string>() { "sanitary" },
                        BrandId = geberitBrand,
                        ShelfId = waterTapShelf
                    },
                    // Shower Mats shelf
                    new Product()
                    {
                        Id = new Guid("8d3f0d22-a35b-4656-b420-00000000008d"),
                        ProductName = "Hansgrohe Antislip Douchemat Wit",
                        Description = "Antislip Douchemat Wit van Hansgrohe.",
                        Barcode = 500141,
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
                        ProductName = "Villeroy & Boch Antislip Douchemat Grijs",
                        Description = "Antislip Douchemat Grijs van Villeroy & Boch.",
                        Barcode = 500142,
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
                        ProductName = "Grohe Antislip Douchemat Zwart",
                        Description = "Antislip Douchemat Zwart van Grohe.",
                        Barcode = 500143,
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
                        ProductName = "Duravit Bamboe Douchemat",
                        Description = "Bamboe Douchemat van Duravit.",
                        Barcode = 500144,
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
                        ProductName = "Geberit Douchemat Rubber Antislip",
                        Description = "Douchemat Rubber Antislip van Geberit.",
                        Barcode = 500145,
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
                        ProductName = "Hansgrohe Douchemat XL Wit",
                        Description = "Douchemat XL Wit van Hansgrohe.",
                        Barcode = 500146,
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
                        ProductName = "Villeroy & Boch Douchemat met Zuignappen",
                        Description = "Douchemat met Zuignappen van Villeroy & Boch.",
                        Barcode = 500147,
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
                        ProductName = "Grohe Douchemat Kunststof Grijs",
                        Description = "Douchemat Kunststof Grijs van Grohe.",
                        Barcode = 500148,
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
                        ProductName = "Duravit Antislip Badmat Blauw",
                        Description = "Antislip Badmat Blauw van Duravit.",
                        Barcode = 500149,
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
                        ProductName = "Geberit Douchemat Antibacterieel Wit",
                        Description = "Douchemat Antibacterieel Wit van Geberit.",
                        Barcode = 500150,
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