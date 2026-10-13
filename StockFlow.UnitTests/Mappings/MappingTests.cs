using StockFlow.Application.Extensions.CashRegister;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Mappings.People;
using StockFlow.Application.Mappings.Stock;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Mappings
{
    public class DifferenceFinderTests
    {
        private class Item
        {
            public string? Name { get; set; }
            public int Amount { get; set; }
            public string? Password { get; set; }
            public byte[]? Image { get; set; }
            public List<string> Tags { get; set; } = [];
        }

        [Fact]
        public void FindDifferences_ChangedValue_ReturnsNewAndOld()
        {
            var result = new Item { Name = "Nieuw" }.FindDifferences(new Item { Name = "Oud" });

            Assert.Equal(["Name: New: Nieuw. Old: Oud"], result);
        }

        [Fact]
        public void FindDifferences_NothingChanged_ReturnsEmpty()
        {
            var result = new Item { Name = "Zelfde", Amount = 2 }.FindDifferences(new Item { Name = "Zelfde", Amount = 2 });

            Assert.Empty(result);
        }

        [Fact]
        public void FindDifferences_NullValue_IsShownAsDash()
        {
            var result = new Item { Name = null }.FindDifferences(new Item { Name = "Oud" });

            Assert.Equal(["Name: New: -. Old: Oud"], result);
        }

        [Fact]
        public void FindDifferences_PasswordChanged_IsNeverReported()
        {
            var result = new Item { Password = "nieuw" }.FindDifferences(new Item { Password = "oud" });

            Assert.Empty(result);
        }

        [Fact]
        public void FindDifferences_ImageChanged_ReportsChangedImage()
        {
            var result = new Item { Image = [1, 2, 3] }.FindDifferences(new Item { Image = [1, 2] });

            Assert.Equal(["Image: Changed Image."], result);
        }

        [Fact]
        public void FindDifferences_TagsChanged_ReportsAddedAndRemoved()
        {
            var result = new Item { Tags = ["tuin", "verf"] }.FindDifferences(new Item { Tags = ["tuin", "hout"] });

            Assert.Equal(["Tags: Added: [verf]. Removed: [hout]"], result);
        }

        [Fact]
        public void WithoutNavigationChanges_NavigationMessage_IsRemoved()
        {
            var result = new[] { "Shelf: New: -. Old: Gereedschap", "Stock: New: 2. Old: 1" }.WithoutNavigationChanges();

            Assert.Equal(["Stock: New: 2. Old: 1"], result);
        }
    }

    public class EnumMappingTests
    {
        [Fact]
        public void LogType_EveryValue_RoundTrips()
        {
            foreach (var value in Enum.GetValues<LogTypeDto>())
                Assert.Equal(value, value.ToDomain().ToDto());
        }

        [Fact]
        public void StockLogType_EveryValue_RoundTrips()
        {
            foreach (var value in Enum.GetValues<StockLogTypeDto>())
                Assert.Equal(value, value.ToDomain().ToDto());
        }

        [Fact]
        public void EmployeeRole_EveryValue_RoundTrips()
        {
            foreach (var value in Enum.GetValues<EmployeeRoleDto>())
                Assert.Equal(value, value.ToDomain().ToDto());
        }

        [Fact]
        public void LogType_UnknownValue_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ((LogTypeDto)99).ToDomain());
        }

        [Fact]
        public void EmployeeRole_UnknownValue_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ((EmployeeRole)99).ToDto());
        }
    }

    public class EntityMappingTests
    {
        [Fact]
        public void ToCashRegisterLogsEntity_Lines_GetReceiptIdAndParentId()
        {
            var dto = new CreateCashRegisterLogsDto
            {
                EmployeeId = Guid.NewGuid(),
                PointsUsed = 100,
                PointsEarned = 4,
                ProductsSold = [new RegisterItemDetailDto { Product = new ProductDto { Id = Guid.NewGuid() }, Amount = 2, PriceAtTimeOfSale = 3m }],
            };

            var entity = dto.ToCashRegisterLogsEntity("9000000007");

            Assert.Equal("9000000007", entity.ReceiptId);
            Assert.Equal(100, entity.PointsUsed);
            Assert.Equal(4, entity.PointsEarned);
            var line = Assert.Single(entity.ProductsSold);
            Assert.Equal(entity.Id, line.CashregisterLogId);
            Assert.Equal(0, line.ReturnedAmount);
        }

        [Fact]
        public void ToRegisterItemDetailEntity_EmptySaleId_StoresNoSale()
        {
            var dto = new RegisterItemDetailDto
            {
                Product = new ProductDto { Id = Guid.NewGuid() },
                Amount = 1,
                Sale = new SalesDto { Id = Guid.Empty, SaleName = "x", Description = "x" },
            };

            var entity = dto.ToRegisterItemDetailEntity();

            Assert.Null(entity.SalesId);
            Assert.Equal(dto.Product.Id, entity.ProductId);
        }

        [Fact]
        public void ToRegisterItemDetailDto_ProductMissing_ReturnsEmptyProduct()
        {
            var entity = new RegisterItemDetail { Id = Guid.NewGuid(), Amount = 1, ReturnedAmount = 1 };

            var dto = entity.ToRegisterItemDetailDto();

            Assert.NotNull(dto.Product);
            Assert.Equal(1, dto.ReturnedAmount);
        }

        [Fact]
        public void ToProductBulkViewDto_ProductOnSale_UsesBestSalePrice()
        {
            var product = TestData.Product(price: 10m);
            product.Sales.Add(TestData.Sale(10));
            product.Sales.Add(TestData.Sale(20));

            var dto = product.ToProductBulkViewDto();

            Assert.Equal(8.00m, dto.Price);
            Assert.True(dto.Sale);
            Assert.Equal("002-3", dto.Location);
            Assert.Equal("Bosch", dto.BrandName);
        }

        [Fact]
        public void ToProductBulkViewDto_NoSale_UsesNormalPrice()
        {
            var dto = TestData.Product(price: 10m).ToProductBulkViewDto();

            Assert.Equal(10m, dto.Price);
            Assert.False(dto.Sale);
        }

        [Fact]
        public void ToCustomerDto_Customer_DoesNotExposePasswordHash()
        {
            var dto = TestData.Customer(savedPoints: 5).ToCustomerDto();

            Assert.Equal(5, dto.SavedPoints);
            Assert.DoesNotContain(typeof(CustomerDto).GetProperties(), p => p.Name.Contains("Password"));
        }

        [Fact]
        public void ChangeStock_Product_ChangesStockAndBuildsLog()
        {
            var product = TestData.Product(stock: 4);
            var userId = Guid.NewGuid();

            var log = product.ChangeStock(-1, userId, LogTypeDto.Sold);
            product.ChangeStock(-1);

            Assert.Equal(3, product.Stock);
            Assert.Equal(product.Barcode.ToString(), log.Identifier);
            Assert.Equal(userId, log.EmployeeId);
            Assert.Contains("Stock: New: 3. Old: 4", log.Messages);
        }
    }
}
