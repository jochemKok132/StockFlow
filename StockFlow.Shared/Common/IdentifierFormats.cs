namespace StockFlow.Shared.Common
{
    public static class IdentifierFormats
    {
        public const int ProductBarcodeLength = 9;
        public const int CustomerIdLength = 8;
        public const char CustomerIdPrefix = '8';
        public const int FirstCustomerId = 80000001;
        public const int ReceiptIdLength = 10;
        public const char ReceiptIdPrefix = '9';
        public const long FirstReceiptId = 9000000001;
        public const long LastReceiptId = 9999999999;
        public const int CustomerOrderNumberLength = 7;
        public const char CustomerOrderNumberPrefix = '7';
        public const int FirstCustomerOrderNumber = 7000001;
        public const int LastCustomerOrderNumber = 7999999;

        public static bool IsEmployeeId(string? input) =>
            input is { Length: FieldLimits.EmployeeId } && input.All(char.IsAsciiDigit);

        public static bool IsProductBarcode(string? input) =>
            input is { Length: ProductBarcodeLength } && input.All(char.IsDigit);

        public static bool IsCustomerId(string? input) =>
            input is { Length: CustomerIdLength } && input[0] == CustomerIdPrefix && input.All(char.IsDigit);

        public static bool IsCustomerOrderNumber(string? input) =>
            input is { Length: CustomerOrderNumberLength } && input[0] == CustomerOrderNumberPrefix && input.All(char.IsDigit);

        public static bool IsReceiptId(string? input) =>
            input is { Length: ReceiptIdLength } && input[0] == ReceiptIdPrefix && input.All(char.IsDigit);
    }
}
