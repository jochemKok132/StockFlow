using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.CashRegister;

namespace StockFlow.Application.Validations.CashRegister
{
    public static class ReturnValidation
    {
        public static void ValidateReceiptId(this string? receiptId)
        {
            if (!IdentifierFormats.IsReceiptId(receiptId))
                throw new ArgumentException(
                    $"A receipt id must be {IdentifierFormats.ReceiptIdLength} digits and start with {IdentifierFormats.ReceiptIdPrefix}.");
        }

        public static void Validate(this ReturnRequest request)
        {
            Guard.NotNull(request, "Return");

            request.ReceiptId.ValidateReceiptId();

            var items = request.Items ?? [];
            if (items.Count == 0)
                throw new ArgumentException("A return needs at least one product.");

            foreach (var item in items)
            {
                if (item.ProductId == Guid.Empty)
                    throw new ArgumentException("Every returned item needs a product.");

                if (item.Amount < 1 || item.Amount > PurchaseValidation.MaxAmountPerLine)
                    throw new ArgumentException($"The amount to return must be between 1 and {PurchaseValidation.MaxAmountPerLine}.");
            }
        }
    }
}
