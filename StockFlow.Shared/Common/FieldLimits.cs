namespace StockFlow.Shared.Common
{
    public static class FieldLimits
    {
        public const int Name = 100;
        public const int Description = 1000;
        public const int Tag = 50;
        public const int Email = 254;
        public const int PostalCode = 7;
        public const int HouseNumber = 10;
        public const int ShelfHallway = 50;
        public const int ShelfLocation = 50;
        public const int EmployeeId = 6;
        public const int EmployeePassword = 12;
        public const int CustomerPassword = 128;
        public const int Quantity = 3;
        public const int MaxImageBytes = 5 * 1024 * 1024;
        public const string TooLongMessage = "{0} can be at most {1} characters.";
    }
}
