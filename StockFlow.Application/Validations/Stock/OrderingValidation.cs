using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Ordering;

namespace StockFlow.Application.Validations.Stock
{
    public static class OrderingValidation
    {
        public const int MaxLinesPerOrder = 50;

        public static void Validate(this AddOrderLineRequest request)
        {
            Guard.NotNull(request, "Order line");
            ValidateBarcode(request.Barcode);
            ValidateAmount(request.Amount);
        }

        public static void Validate(this UpdateOrderLineRequest request)
        {
            Guard.NotNull(request, "Order line");
            Guard.NotEmpty(request.LineId, "Order line");
            if (request.Amount < 0 || request.Amount > FieldLimits.MaxOrderAmount)
                throw new ArgumentException($"The amount must be between 0 and {FieldLimits.MaxOrderAmount}.");
        }

        public static void Validate(this ReceiveOrderRequest request)
        {
            Guard.NotNull(request, "Delivery");
            if (request.Lines is null || request.Lines.Count == 0)
                throw new ArgumentException("Enter what was delivered.");
            if (request.Lines.Select(l => l.LineId).Distinct().Count() != request.Lines.Count)
                throw new ArgumentException("Every order line can only be received once.");
            foreach (var line in request.Lines)
            {
                Guard.NotEmpty(line.LineId, "Order line");
                if (line.ReceivedAmount < 0 || line.ReceivedAmount > FieldLimits.MaxOrderAmount)
                    throw new ArgumentException($"The received amount must be between 0 and {FieldLimits.MaxOrderAmount}.");
            }
        }

        public static void Validate(this CreateCustomerOrderRequest request)
        {
            Guard.NotNull(request, "Customer order");
            if (!IdentifierFormats.IsCustomerId(request.CustomerId?.Trim()))
                throw new ArgumentException($"A customer id must be {IdentifierFormats.CustomerIdLength} digits and start with {IdentifierFormats.CustomerIdPrefix}.");
            Guard.Optional(request.Note, "Note", FieldLimits.OrderNote);
            if (request.Lines is null || request.Lines.Count == 0)
                throw new ArgumentException("A customer order needs at least one product.");
            if (request.Lines.Count > MaxLinesPerOrder)
                throw new ArgumentException($"A customer order can have at most {MaxLinesPerOrder} lines.");
            foreach (var line in request.Lines)
            {
                ValidateBarcode(line.Barcode);
                ValidateAmount(line.Amount);
            }
        }

        public static void Validate(this CreateStockCountRequest request)
        {
            Guard.NotNull(request, "Stock count");
            Guard.NotEmpty(request.ShelfId, "Shelf");
            Guard.Optional(request.Note, "Note", FieldLimits.OrderNote);
        }

        public static void Validate(this CompleteStockCountRequest request)
        {
            Guard.NotNull(request, "Stock count");
            if (request.Lines is null || request.Lines.Count == 0)
                throw new ArgumentException("Enter the counted stock.");
            if (request.Lines.Select(l => l.LineId).Distinct().Count() != request.Lines.Count)
                throw new ArgumentException("Every product can only be counted once.");
            foreach (var line in request.Lines)
            {
                Guard.NotEmpty(line.LineId, "Count line");
                if (line.CountedStock < 0 || line.CountedStock > FieldLimits.MaxStockLevel)
                    throw new ArgumentException($"The counted stock must be between 0 and {FieldLimits.MaxStockLevel}.");
                if (line.ExpiredAmount < 0 || line.ExpiredAmount > FieldLimits.MaxStockLevel)
                    throw new ArgumentException($"The expired amount must be between 0 and {FieldLimits.MaxStockLevel}.");
            }
        }

        private static void ValidateBarcode(int barcode)
        {
            if (!IdentifierFormats.IsProductBarcode(barcode.ToString()))
                throw new ArgumentException($"A product barcode must be {IdentifierFormats.ProductBarcodeLength} digits.");
        }

        private static void ValidateAmount(int amount)
        {
            if (amount < 1 || amount > FieldLimits.MaxOrderAmount)
                throw new ArgumentException($"The amount must be between 1 and {FieldLimits.MaxOrderAmount}.");
        }
    }
}
