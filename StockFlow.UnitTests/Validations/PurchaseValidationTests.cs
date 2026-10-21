using StockFlow.Application.Validations.CashRegister;
using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Stock.Sales;

namespace StockFlow.UnitTests.Validations
{
    public class PurchaseValidationTests
    {
        private static RegisterItemDetailDto Line(decimal price, int amount, SalesDto? sale = null) => new()
        {
            Product = new ProductDto { Id = Guid.NewGuid(), ProductName = "Hammer" },
            Amount = amount,
            PriceAtTimeOfSale = price,
            Sale = sale,
        };

        private static Purchase CreatePurchase(Guid? customerId, int pointsUsed, params RegisterItemDetailDto[] lines) => new()
        {
            CustomerId = customerId,
            PointsUsed = pointsUsed,
            TotalPrice = (double)lines.Sum(l => l.PriceAtTimeOfSale * l.Amount),
            ProductsSold = lines,
        };

        [Fact]
        public void Validate_NoLines_ThrowsArgumentException()
        {
            var purchase = CreatePurchase(null, 0);

            Assert.Throws<ArgumentException>(() => purchase.Validate(false, 0));
        }

        [Fact]
        public void Validate_WalkInPurchase_DoesNotThrow()
        {
            var purchase = CreatePurchase(null, 0, Line(5m, 2));

            var exception = Record.Exception(() => purchase.Validate(false, 0));

            Assert.Null(exception);
        }

        [Fact]
        public void Validate_SaleWithoutCustomer_ThrowsArgumentException()
        {
            var sale = new SalesDto { SaleName = "Summer", Description = "Summer sale", PercentageOff = 20 };
            var purchase = CreatePurchase(null, 0, Line(4m, 1, sale));

            Assert.Throws<ArgumentException>(() => purchase.Validate(false, 0));
        }

        [Fact]
        public void Validate_EndedSale_ThrowsArgumentException()
        {
            var today = new DateTime(2026, 10, 20);
            var sale = new SalesDto { SaleName = "Summer", Description = "Summer sale", PercentageOff = 20, EndDate = today.AddDays(-1) };
            var purchase = CreatePurchase(Guid.NewGuid(), 0, Line(4m, 1, sale));

            Assert.Throws<ArgumentException>(() => purchase.Validate(true, 0, today: today));
        }

        [Fact]
        public void Validate_ScheduledSale_ThrowsArgumentException()
        {
            var today = new DateTime(2026, 10, 20);
            var sale = new SalesDto { SaleName = "Winter", Description = "Winter sale", PercentageOff = 20, StartDate = today.AddDays(1) };
            var purchase = CreatePurchase(Guid.NewGuid(), 0, Line(4m, 1, sale));

            Assert.Throws<ArgumentException>(() => purchase.Validate(true, 0, today: today));
        }

        [Fact]
        public void Validate_MovingSaleWithoutActiveMovingSale_ThrowsArgumentException()
        {
            var today = new DateTime(2026, 10, 20);
            var sale = new SalesDto { Id = SalePricing.MovingSaleId, SaleName = "Moving sale", Description = "x", SaleType = SaleTypeDto.Moving, PercentageOff = 10 };
            var purchase = CreatePurchase(Guid.NewGuid(), 0, Line(9m, 1, sale));

            Assert.Throws<ArgumentException>(() => purchase.Validate(true, 0, movingSaleUntil: today.AddDays(-1), today: today));
        }

        [Fact]
        public void Validate_MovingSaleWithActiveMovingSale_DoesNotThrow()
        {
            var today = new DateTime(2026, 10, 20);
            var sale = new SalesDto { Id = SalePricing.MovingSaleId, SaleName = "Moving sale", Description = "x", SaleType = SaleTypeDto.Moving, PercentageOff = 10 };
            var purchase = CreatePurchase(Guid.NewGuid(), 0, Line(9m, 1, sale));

            Assert.Null(Record.Exception(() => purchase.Validate(true, 0, movingSaleUntil: today.AddDays(30), today: today)));
        }

        [Fact]
        public void Validate_PointsWithoutCustomer_ThrowsArgumentException()
        {
            var purchase = CreatePurchase(null, 100, Line(10m, 1));

            Assert.Throws<ArgumentException>(() => purchase.Validate(false, 500));
        }

        [Fact]
        public void Validate_PartialPoints_ThrowsArgumentException()
        {
            var purchase = CreatePurchase(Guid.NewGuid(), 100, Line(10m, 1));

            Assert.Throws<ArgumentException>(() => purchase.Validate(true, 500));
        }

        [Fact]
        public void Validate_AllAvailablePoints_DoesNotThrow()
        {
            var purchase = CreatePurchase(Guid.NewGuid(), 500, Line(10m, 1));

            var exception = Record.Exception(() => purchase.Validate(true, 500));

            Assert.Null(exception);
        }

        [Fact]
        public void Validate_PointsCappedAtReceiptTotal_DoesNotThrow()
        {
            var purchase = CreatePurchase(Guid.NewGuid(), 840, Line(8.40m, 1));

            var exception = Record.Exception(() => purchase.Validate(true, 1250));

            Assert.Null(exception);
        }

        [Fact]
        public void Validate_MorePointsThanCustomerHas_ThrowsArgumentException()
        {
            var purchase = CreatePurchase(Guid.NewGuid(), 600, Line(10m, 1));

            Assert.Throws<ArgumentException>(() => purchase.Validate(true, 500));
        }
    }
}
