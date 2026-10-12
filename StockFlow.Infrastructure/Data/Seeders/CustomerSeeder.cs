using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.People;

namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class CustomerSeeder
    {
        private const string SeedPassword = "$2a$12$JQPzgasE5YB5MKx8PxKDfegztZWYHS1IYikQkx/u7ONXTaF1FyAzq";

        public static void UseCustomerSeeder(this DbContext context)
        {
            if (context.Set<Customer>().Any())
                return;

            var now = DateTime.UtcNow;

            IEnumerable<Customer> customers = new List<Customer>()
            {
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000001"),
                        CustomerId = "80000001",
                        FullName = "Sanne de Vries",
                        Email = "sanne.devries@example.nl",
                        PostalCode = "3511AB",
                        HouseNumber = "12",
                        HashedPassword = SeedPassword,
                        SavedPoints = 0,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000002"),
                        CustomerId = "80000002",
                        FullName = "Daan Jansen",
                        Email = "daan.jansen@example.nl",
                        PostalCode = "1012JS",
                        HouseNumber = "4",
                        HashedPassword = SeedPassword,
                        SavedPoints = 0,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000003"),
                        CustomerId = "80000003",
                        FullName = "Emma Bakker",
                        Email = "emma.bakker@example.nl",
                        PostalCode = "2511CV",
                        HouseNumber = "88",
                        HashedPassword = SeedPassword,
                        SavedPoints = 120,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000004"),
                        CustomerId = "80000004",
                        FullName = "Lucas Visser",
                        Email = "lucas.visser@example.nl",
                        PostalCode = "3011AD",
                        HouseNumber = "23A",
                        HashedPassword = SeedPassword,
                        SavedPoints = 50,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000005"),
                        CustomerId = "80000005",
                        FullName = "Julia Smit",
                        Email = "julia.smit@example.nl",
                        PostalCode = "5611AZ",
                        HouseNumber = "7",
                        HashedPassword = SeedPassword,
                        SavedPoints = 50,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000006"),
                        CustomerId = "80000006",
                        FullName = "Sem Meijer",
                        Email = "sem.meijer@example.nl",
                        PostalCode = "9711LM",
                        HouseNumber = "150",
                        HashedPassword = SeedPassword,
                        SavedPoints = 25,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000007"),
                        CustomerId = "80000007",
                        FullName = "Tess de Boer",
                        Email = "tess.deboer@example.nl",
                        PostalCode = "6811KL",
                        HouseNumber = "3",
                        HashedPassword = SeedPassword,
                        SavedPoints = 0,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000008"),
                        CustomerId = "80000008",
                        FullName = "Finn Mulder",
                        Email = "finn.mulder@example.nl",
                        PostalCode = "7511JE",
                        HouseNumber = "41",
                        HashedPassword = SeedPassword,
                        SavedPoints = 75,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000009"),
                        CustomerId = "80000009",
                        FullName = "Lotte de Groot",
                        Email = "lotte.degroot@example.nl",
                        PostalCode = "8011PK",
                        HouseNumber = "9B",
                        HashedPassword = SeedPassword,
                        SavedPoints = 0,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000010"),
                        CustomerId = "80000010",
                        FullName = "Milan Bos",
                        Email = "milan.bos@example.nl",
                        PostalCode = "4811ZH",
                        HouseNumber = "62",
                        HashedPassword = SeedPassword,
                        SavedPoints = 10,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000011"),
                        CustomerId = "80000011",
                        FullName = "Noor Vos",
                        Email = "noor.vos@example.nl",
                        PostalCode = "2011LS",
                        HouseNumber = "15",
                        HashedPassword = SeedPassword,
                        SavedPoints = 310,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000012"),
                        CustomerId = "80000012",
                        FullName = "Bram Peters",
                        Email = "bram.peters@example.nl",
                        PostalCode = "1211AB",
                        HouseNumber = "2",
                        HashedPassword = SeedPassword,
                        SavedPoints = 0,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000013"),
                        CustomerId = "80000013",
                        FullName = "Fleur Hendriks",
                        Email = "fleur.hendriks@example.nl",
                        PostalCode = "5211DB",
                        HouseNumber = "33",
                        HashedPassword = SeedPassword,
                        SavedPoints = 0,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000014"),
                        CustomerId = "80000014",
                        FullName = "Thijs van Dijk",
                        Email = "thijs.vandijk@example.nl",
                        PostalCode = "6511HW",
                        HouseNumber = "19",
                        HashedPassword = SeedPassword,
                        SavedPoints = 0,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
                    new Customer()
                    {
                        Id = new Guid("c5a1e000-0000-4000-8000-000000000015"),
                        CustomerId = "80000015",
                        FullName = "Isa Kok",
                        Email = "isa.kok@example.nl",
                        PostalCode = "1501AA",
                        HouseNumber = "8",
                        HashedPassword = SeedPassword,
                        SavedPoints = 50,
                        CreatedAt = now,
                        UpdatedAt = now,
                    },
            };

            context.Set<Customer>().AddRange(customers);
            context.SaveChanges();
        }
    }
}
