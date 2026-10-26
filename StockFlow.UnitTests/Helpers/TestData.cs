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
            ShelfName = "Tools",
            ShelfLocation = "002",
            ShelfHallway = "A",
            SoftDeleted = softDeleted,
        };

        public static Sales Sale(int percentageOff = 20, params string[] tags) => new()
        {
            Id = Guid.NewGuid(),
            SaleName = "Summer",
            Description = "Summer discount",
            PercentageOff = percentageOff,
            SalesTags = tags.Length == 0 ? ["garden"] : tags,
        };

        public static Supplier Supplier(bool softDeleted = false, DayOfWeek orderDay = DayOfWeek.Monday) => new()
        {
            Id = Guid.NewGuid(),
            SupplierName = "Wholesale",
            OrderDay = orderDay,
            LeadTimeDays = 2,
            SoftDeleted = softDeleted,
        };

        public static Product Product(int stock = 10, decimal price = 10m, ProductBrand? brand = null, Shelf? shelf = null, Supplier? supplier = null)
        {
            brand ??= Brand();
            shelf ??= Shelf();
            supplier ??= Supplier();
            return new Product
            {
                Id = Guid.NewGuid(),
                ProductName = "Hammer",
                Description = "Claw Hammer",
                Price = price,
                Barcode = 500000001,
                Stock = stock,
                Location = 3,
                SalesTags = ["tools"],
                BrandId = brand.Id,
                Brand = brand,
                ShelfId = shelf.Id,
                Shelf = shelf,
                SupplierId = supplier.Id,
                Supplier = supplier,
            };
        }

        public static CreateProductDto CreateProductDto(Guid brandId, Guid shelfId) => new()
        {
            ProductName = "Hammer",
            Description = "Claw Hammer",
            Price = 12.99m,
            Stock = 5,
            Location = 3,
            SalesTags = ["tools"],
            BrandId = brandId,
            ShelfId = shelfId,
            SupplierId = StockFlow.Domain.Entities.Stock.Supplier.DefaultSupplierId,
        };

        public static UpdateProductDto UpdateProductDto(Product product, params string[] tags) => new()
        {
            Id = product.Id,
            ProductName = "Hammer XL",
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Location = product.Location,
            SalesTags = tags,
            BrandId = product.BrandId,
            ShelfId = product.ShelfId,
            SupplierId = product.SupplierId,
            Status = (ProductStatusDto)(int)product.Status,
            MinimumStock = product.MinimumStock,
            MaximumStock = product.MaximumStock,
            Orderable = product.Orderable,
            AutoOrder = product.AutoOrder,
            CanExpire = product.CanExpire,
        };

        public static RegisterRequest RegisterRequest() => new()
        {
            Name = "  Jan Jansen ",
            Email = " Jan@Example.com ",
            Password = "Welcome123!",
            ConfirmPassword = "Welcome123!",
            PostalCode = "1234ab",
            HouseNumber = "12a",
        };

        public static ProductPaginationDto ProductPagination(int pageSize = 10) => new() { PageNumber = 1, PageSize = pageSize };
    }
}
