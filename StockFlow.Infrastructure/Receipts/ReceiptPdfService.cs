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
        private static readonly CultureInfo Dutch = CultureInfo.GetCultureInfo("nl-NL");
        private const float BarcodeWidthMm = 60;
        private const float BarcodeHeightMm = 14;

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

                        Pair(column, "Bon", receipt.ReceiptId);
                        Pair(column, "Datum", LocalTime(receipt.CreatedAt, settings.TimeZone).ToString("dd-MM-yyyy HH:mm", Dutch));
                        if (receipt.Employee is not null)
                            Pair(column, "Medewerker", receipt.Employee.EmployeeId);
                        if (receipt.Customer is not null)
                            Pair(column, "Klant", $"{receipt.Customer.FullName} ({receipt.Customer.CustomerId})");

                        Divider(column);

                        foreach (var line in lines)
                        {
                            string name = line.Product?.ProductName ?? "Onbekend product";
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Text(name).SemiBold();
                                row.ConstantItem(55).AlignRight().Text(Money(SalePricing.Total(line.PriceAtTimeOfSale, line.Amount)));
                            });
                            column.Item().PaddingLeft(6).Text($"{line.Amount} x {Money(line.PriceAtTimeOfSale)}").FontColor(Colors.Grey.Darken2);

                            if (line.Sale is not null)
                                column.Item().PaddingLeft(6).Text($"{SalePricing.Describe(line.Sale)} {line.Sale.SaleName}").FontColor(Colors.Grey.Darken2);

                            if (line.ReturnedAmount > 0)
                                column.Item().PaddingLeft(6).Text($"{line.ReturnedAmount} retour").FontColor(Colors.Grey.Darken2);
                        }

                        Divider(column);

                        Pair(column, "Artikelen", Money(linesTotal));
                        if (discount > 0)
                            Pair(column, "Waarvan korting", Money(discount));
                        if (receipt.PointsUsed > 0)
                            Pair(column, $"Punten ({receipt.PointsUsed})", $"-{Money(pointsValue)}");

                        column.Item().PaddingTop(2).Row(row =>
                        {
                            row.RelativeItem().Text("TOTAAL").Bold().FontSize(11);
                            row.ConstantItem(70).AlignRight().Text(Money(total)).Bold().FontSize(11);
                        });

                        if (vat > 0)
                            Pair(column, $"Btw {settings.VatPercentage.ToString("0.##", Dutch)}% (inbegrepen)", Money(vat));
                        if (receipt.PointsEarned > 0)
                            Pair(column, "Gespaarde punten", receipt.PointsEarned.ToString(Dutch));

                        Divider(column);

                        column.Item().AlignCenter().Text("Bedankt voor uw aankoop!");
                        column.Item().AlignCenter().Text("Bewaar deze bon voor ruilen of retour.").FontColor(Colors.Grey.Darken2);

                        column.Item().PaddingTop(8).AlignCenter().Width(BarcodeWidthMm, Unit.Millimetre).Height(BarcodeHeightMm, Unit.Millimetre)
                            .Svg(Code128.ToSvg(receipt.ReceiptId, 2, 60));
                        column.Item().AlignCenter().Text(receipt.ReceiptId).FontSize(8).LetterSpacing(0.2f);
                    });
                });
            }).GeneratePdf();
        }

        private static void Pair(ColumnDescriptor column, string label, string value) =>
            column.Item().Row(row =>
            {
                row.RelativeItem().Text(label);
                row.AutoItem().AlignRight().Text(value);
            });

        private static void Divider(ColumnDescriptor column) =>
            column.Item().PaddingVertical(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);

        private static string Money(decimal value) => value.ToString("C", Dutch);

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
