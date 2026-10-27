using System.Text;
using Microsoft.Extensions.Options;
using StockFlow.Infrastructure.Receipts;

namespace StockFlow.UnitTests.Infrastructure
{
    public class ReceiptPdfServiceTests
    {
        [Fact]
        public void BarcodeWidth_ReceiptId_KeepsTheSvgAspectRatioWithinThePaper()
        {
            float width = ReceiptPdfService.BarcodeWidth("9000000001");

            Assert.Equal(14f * Code128.SvgWidth("9000000001", 2) / 60, width, 3);
            Assert.True(width <= 66f);
        }

        [Fact]
        public void BarcodeWidth_VeryLongValue_IsCappedAtThePaperWidth()
        {
            Assert.Equal(66f, ReceiptPdfService.BarcodeWidth(new string('A', 40)));
        }

        [Fact]
        public void Create_Receipt_ReturnsAPdf()
        {
            var sut = new ReceiptPdfService(Options.Create(new ReceiptOptions { StoreName = "StockFlow Hardware Store", AddressLines = ["Test Street 1"] }));
            var receipt = new CashRegisterLogsDto
            {
                ReceiptId = "9000000001",
                CreatedAt = new DateTime(2026, 10, 20, 12, 0, 0, DateTimeKind.Utc),
                TotalPrice = 12.5,
                TotalOff = 2.5,
                PointsUsed = 100,
                PointsEarned = 12,
                Customer = new CustomerDto { CustomerId = "80000001", FullName = "Jan Jansen", Email = "jan@example.com" },
                Employee = new EmployeeDto { EmployeeId = "000010", FullName = "Piet" },
                ProductsSold =
                [
                    new RegisterItemDetailDto
                    {
                        Product = new ProductDto { ProductName = "Hammer", Price = 5m },
                        Amount = 3,
                        ReturnedAmount = 1,
                        PriceAtTimeOfSale = 3.333333m,
                        Sale = new SalesDto { SaleName = "Tools", Description = "x", SaleType = SaleTypeDto.MultiBuy, BuyAmount = 2, FreeAmount = 1 },
                    },
                ],
            };

            var result = sut.Create(receipt);

            Assert.True(result.Length > 1000);
            Assert.Equal("%PDF", Encoding.ASCII.GetString(result, 0, 4));
        }
    }
}
