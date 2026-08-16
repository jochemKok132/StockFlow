
using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.Stock;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class ShelfSeeder
    {
        public static void UseShelfSeeder(this DbContext context)
        {
            if (!context.Set<Shelf>().Any())
            {
                IEnumerable<Shelf> Shelfs = new List<Shelf>()
                {
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde3"),
                        ShelfName = "Flexa",
                        ShelfHallway = "Paint",
                        ShelfLocation = "001"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde4"),
                        ShelfName = "Sigma",
                        ShelfHallway = "Paint",
                        ShelfLocation = "002"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde5"),
                        ShelfName = "Histor",
                        ShelfHallway = "Paint",
                        ShelfLocation = "003"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde6"),
                        ShelfName = "Screws",
                        ShelfHallway = "Hardware",
                        ShelfLocation = "004"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde7"),
                        ShelfName = "Nails",
                        ShelfHallway = "Hardware",
                        ShelfLocation = "005"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b420-ed3e6398cde5"),
                        ShelfName = "Locks",
                        ShelfHallway = "Hardware",
                        ShelfLocation = "006"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b420-ed3e5398cde5"),
                        ShelfName = "Sockets",
                        ShelfHallway = "Electronics",
                        ShelfLocation = "007"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b420-ed3e4398cde5"),
                        ShelfName = "Plugs",
                        ShelfHallway = "Electronics",
                        ShelfLocation = "008"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b420-ed3e3398cde5"),
                        ShelfName = "Wires",
                        ShelfHallway = "Electronics",
                        ShelfLocation = "009"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b430-ed3e3398cde5"),
                        ShelfName = "Planks",
                        ShelfHallway = "Wood",
                        ShelfLocation = "010"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b440-ed3e3398cde5"),
                        ShelfName = "Beams",
                        ShelfHallway = "Wood",
                        ShelfLocation = "011"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b450-ed3e3398cde5"),
                        ShelfName = "Impregnated Wood",
                        ShelfHallway = "Wood",
                        ShelfLocation = "012"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b530-ed3e3398cde5"),
                        ShelfName = "Showers",
                        ShelfHallway = "Sanitary",
                        ShelfLocation = "013"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b630-ed3e3398cde5"),
                        ShelfName = "Water Tap",
                        ShelfHallway = "Sanitary",
                        ShelfLocation = "014"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b330-ed3e3398cde5"),
                        ShelfName = "Shower Mats",
                        ShelfHallway = "Sanitary",
                        ShelfLocation = "015"
                    },
                };
                context.Set<Shelf>().AddRange(Shelfs);
                context.SaveChanges();
            }
        }
    }
}