        
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
                        ShelfLocation = "01"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde4"),
                        ShelfName = "Sigma",
                        ShelfHallway = "Paint",
                        ShelfLocation = "02"
                    },
                    new Shelf()
                    {
                        Id = new Guid("55ad0d22-a35b-4656-b420-ed3e7398cde5"),
                        ShelfName = "Histor",
                        ShelfHallway = "Paint",
                        ShelfLocation = "03"
                    },
                }
                context.Set<Shelf>().AddRange(Shelfs);
                context.SaveChanges();
            }
        }
    }
}