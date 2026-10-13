namespace StockFlow.UnitTests.Helpers
{
    public static class TestData
    {
        public static Customer Customer(int savedPoints = 0, string hashedPassword = "hash", bool softDeleted = false) => new()
        {
            Id = Guid.NewGuid(),
            CustomerId = "80000001",
            FullName = "Jan Jansen",
            Email = "jan@example.com",
            PostalCode = "1234 AB",
            HouseNumber = "12",
            HashedPassword = hashedPassword,
            SavedPoints = savedPoints,
            SoftDeleted = softDeleted,
        };

        public static Employee Employee(EmployeeRole role = EmployeeRole.Cashier, string? hashedPassword = "hash") => new()
        {
            Id = Guid.NewGuid(),
            EmployeeId = "000010",
            FullName = "Piet Pieters",
            HashedPassword = hashedPassword,
            Role = role,
        };

        public static ProductBrand Brand(bool softDeleted = false) => new()
        {
            Id = Guid.NewGuid(),
            BrandName = "Bosch",
            SoftDeleted = softDeleted,
        };

        public static Shelf Shelf(bool softDeleted = false) => new()
        {
            Id = Guid.NewGuid(),
            ShelfName = "Gereedschap",
            ShelfLocation = "002",
            ShelfHallway = "A",
            SoftDeleted = softDeleted,
        };

        public static Sales Sale(int percentageOff = 20, params string[] tags) => new()
        {
            Id = Guid.NewGuid(),
            SaleName = "Zomer",
            Description = "Zomerkorting",
            PercentageOff = percentageOff,
            SalesTags = tags.Length == 0 ? ["tuin"] : tags,
        };

        public static Product Product(int stock = 10, decimal price = 10m, ProductBrand? brand = null, Shelf? shelf = null)
        {
            brand ??= Brand();
            shelf ??= Shelf();
            return new Product
            {
                Id = Guid.NewGuid(),
                ProductName = "Hamer",
                Description = "Klauwhamer",
                Price = price,
                Barcode = 500000001,
                Stock = stock,
                Location = 3,
                SalesTags = ["gereedschap"],
                BrandId = brand.Id,
                Brand = brand,
                ShelfId = shelf.Id,
                Shelf = shelf,
            };
        }

        public static CreateProductDto CreateProductDto(Guid brandId, Guid shelfId) => new()
        {
            ProductName = "Hamer",
            Description = "Klauwhamer",
            Price = 12.99m,
            Stock = 5,
            Location = 3,
            SalesTags = ["gereedschap"],
            BrandId = brandId,
            ShelfId = shelfId,
        };

        public static UpdateProductDto UpdateProductDto(Product product, params string[] tags) => new()
        {
            Id = product.Id,
            ProductName = "Hamer XL",
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Location = product.Location,
            SalesTags = tags,
            BrandId = product.BrandId,
            ShelfId = product.ShelfId,
        };

        public static RegisterRequest RegisterRequest() => new()
        {
            Name = "  Jan Jansen ",
            Email = " Jan@Example.com ",
            Password = "Welkom123!",
            ConfirmPassword = "Welkom123!",
            PostalCode = "1234ab",
            HouseNumber = "12a",
        };

        public static ProductPaginationDto ProductPagination(int pageSize = 10) => new() { PageNumber = 1, PageSize = pageSize };
    }
}
