using System.Globalization;
using System.Text.RegularExpressions;

namespace StockFlow.Shared.Common
{
    public static partial class StockLogMessages
    {
        public const string ReceiptPrefix = "Receipt: ";

        [GeneratedRegex(@"^Stock: New: (?<new>-?\d+)\. Old: (?<old>-?\d+)$")]
        private static partial Regex StockPattern();

        public static bool TryParseStock(string message, out int newStock, out int oldStock)
        {
            newStock = oldStock = 0;
            var match = StockPattern().Match(message);
            if (!match.Success) return false;
            newStock = int.Parse(match.Groups["new"].Value, CultureInfo.InvariantCulture);
            oldStock = int.Parse(match.Groups["old"].Value, CultureInfo.InvariantCulture);
            return true;
        }

        public static int? StockChange(IEnumerable<string> messages)
        {
            foreach (var message in messages)
                if (TryParseStock(message, out int newStock, out int oldStock))
                    return newStock - oldStock;
            return null;
        }

        public static string? ReceiptId(IEnumerable<string> messages) =>
            messages.FirstOrDefault(m => m.StartsWith(ReceiptPrefix, StringComparison.Ordinal))?[ReceiptPrefix.Length..].Trim();
    }
}
