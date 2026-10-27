using System.Globalization;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StockFlow.Application.Interfaces;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.AuditLogs.CashRegister;

namespace StockFlow.Infrastructure.Receipts
{
    public class ReceiptPdfService(IOptions<ReceiptOptions> options) : IReceiptPdfService
    {
        private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("en-IE");
        private const float MaxBarcodeWidthMm = 66;
        private const float BarcodeHeightMm = 14;
        private const int BarcodeModuleWidth = 2;
        private const int BarcodeSvgHeight = 60;

        static ReceiptPdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] Create(CashRegisterLogsDto receipt)
        {
            var settings = options.Value;
            var lines = receipt.ProductsSold.ToList();
            decimal linesTotal = lines.Sum(l => SalePricing.Total(l.PriceAtTimeOfSale, l.Amount));
            decimal pointsValue = LoyaltyPoints.ToEuros(receipt.PointsUsed);
            decimal total = (decimal)receipt.TotalPrice;
            decimal discount = (decimal)receipt.TotalOff;
            decimal vat = settings.VatPercentage <= 0 ? 0 : Math.Round(total - total / (1 + settings.VatPercentage / 100m), 2);

            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.ContinuousSize(80, Unit.Millimetre);
                    page.Margin(5, Unit.Millimetre);
                    page.DefaultTextStyle(text => text.FontSize(8).FontColor(Colors.Black));

                    page.Content().Column(column =>
                    {
                        column.Spacing(2);

                        column.Item().AlignCenter().Text(settings.StoreName).Bold().FontSize(13);
                        foreach (var addressLine in settings.AddressLines)
                            column.Item().AlignCenter().Text(addressLine).FontColor(Colors.Grey.Darken2);

                        Divider(column);

                        Pair(column, "Receipt", receipt.ReceiptId);
                        Pair(column, "Date", LocalTime(receipt.CreatedAt, settings.TimeZone).ToString("dd-MM-yyyy HH:mm", Culture));
                        if (receipt.Employee is not null)
                            Pair(column, "Employee", receipt.Employee.EmployeeId);
                        if (receipt.Customer is not null)
                            Pair(column, "Customer", $"{receipt.Customer.FullName} ({receipt.Customer.CustomerId})");

                        Divider(column);

                        foreach (var line in lines)
                        {
                            string name = line.Product?.ProductName ?? "Unknown product";
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text(name).SemiBold();
                                row.ConstantItem(55).AlignRight().Text(Money(SalePricing.Total(line.PriceAtTimeOfSale, line.Amount)));
                            });
                            column.Item().PaddingLeft(6).Text($"{line.Amount} x {Money(line.PriceAtTimeOfSale)}").FontColor(Colors.Grey.Darken2);

                            if (line.Sale is not null)
                                column.Item().PaddingLeft(6).Text($"{SalePricing.Describe(line.Sale)} {line.Sale.SaleName}").FontColor(Colors.Grey.Darken2);

                            if (line.ReturnedAmount > 0)
                                column.Item().PaddingLeft(6).Text($"{line.ReturnedAmount} returned").FontColor(Colors.Grey.Darken2);
                        }

                        Divider(column);

                        Pair(column, "Items", Money(linesTotal));
                        if (discount > 0)
                            Pair(column, "Of which discount", Money(discount));
                        if (receipt.PointsUsed > 0)
                            Pair(column, $"Points ({receipt.PointsUsed})", $"-{Money(pointsValue)}");

                        column.Item().PaddingTop(2).Row(row =>
                        {
                            row.RelativeItem().Text("TOTAL").Bold().FontSize(11);
                            row.ConstantItem(70).AlignRight().Text(Money(total)).Bold().FontSize(11);
                        });

                        if (vat > 0)
                            Pair(column, $"VAT {settings.VatPercentage.ToString("0.##", Culture)}% (included)", Money(vat));
                        if (receipt.PointsEarned > 0)
                            Pair(column, "Points earned", receipt.PointsEarned.ToString(Culture));

                        Divider(column);

                        column.Item().AlignCenter().Text("Thank you for your purchase!");
                        column.Item().AlignCenter().Text("Keep this receipt for exchanges or returns.").FontColor(Colors.Grey.Darken2);

                        column.Item().PaddingTop(8).AlignCenter().Width(BarcodeWidth(receipt.ReceiptId), Unit.Millimetre).Height(BarcodeHeightMm, Unit.Millimetre)
                            .Svg(Code128.ToSvg(receipt.ReceiptId, BarcodeModuleWidth, BarcodeSvgHeight));
                        column.Item().AlignCenter().Text(receipt.ReceiptId).FontSize(8).LetterSpacing(0.2f);
                    });
                });
            }).GeneratePdf();
        }

        public static float BarcodeWidth(string receiptId) =>
            Math.Min(MaxBarcodeWidthMm, BarcodeHeightMm * Code128.SvgWidth(receiptId, BarcodeModuleWidth) / BarcodeSvgHeight);

        private static void Pair(ColumnDescriptor column, string label, string value) =>
            column.Item().Row(row =>
            {
                row.RelativeItem().Text(label);
                row.AutoItem().AlignRight().Text(value);
            });

        private static void Divider(ColumnDescriptor column) =>
            column.Item().PaddingVertical(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);

        private static string Money(decimal value) => value.ToString("C", Culture);

        private static DateTime LocalTime(DateTime utc, string timeZone)
        {
            var value = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            try
            {
                return TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.FindSystemTimeZoneById(timeZone));
            }
            catch (Exception exception) when (exception is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                return value.ToLocalTime();
            }
        }
    }
}
