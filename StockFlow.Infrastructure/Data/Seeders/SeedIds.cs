namespace StockFlow.Infrastructure.Data.Seeders
{
    public static class SeedIds
    {
        public static readonly Guid Admin = new("7be9b779-ac76-440f-ad76-1251ff1dfb57");
        public static readonly Guid Manager = new("7be9b779-ac76-440f-ad76-1251ff1efb57");
        public static readonly Guid ShiftLeaderOne = new("7be9b779-ac76-440f-ad76-1251ff1efb56");
        public static readonly Guid ServiceDeskOne = new("7be9b779-ac76-440f-ad76-1251ff1efb53");
        public static readonly Guid CashierOne = new("7be9b779-ac76-440f-ad76-1251ff1efb51");
        public static readonly Guid CashierTwo = new("7be9b779-ac76-440f-ad76-1251ff1efb47");
        public static readonly Guid FloorWorkerOne = new("7be9b779-ac76-440f-ad76-1251ff1efb45");
        public static readonly Guid FloorWorkerTwo = new("7be9b779-ac76-440f-ad76-1251ff1efb44");
        public static readonly Guid Support = new("7be9b779-ac76-440f-ad76-1251ff1efb62");

        public static readonly Guid ScrewsShelf = new("55ad0d22-a35b-4656-b420-ed3e7398cde6");
        public static readonly Guid LocksShelf = new("55ad0d22-a35b-4656-b420-ed3e6398cde5");

        public static readonly Guid PickedUpCustomerOrder = new("c0de0d22-a35b-4656-b420-000000000001");
        public static readonly Guid ReadyCustomerOrder = new("c0de0d22-a35b-4656-b420-000000000002");
        public static readonly Guid WaitingCustomerOrder = new("c0de0d22-a35b-4656-b420-000000000003");
        public static readonly Guid CancelledCustomerOrder = new("c0de0d22-a35b-4656-b420-000000000004");

        public static readonly Guid PickupReceipt = new("c0de0d22-a35b-4656-b420-0000000000a1");

        public static Guid Customer(int number) => new($"c5a1e000-0000-4000-8000-{number:D12}");

        public static Guid Product(int barcode) => new($"8d3f0d22-a35b-4656-b420-{barcode - 500000000:x12}");

        public static DateTime DaysAgo(int days, int hour = 12, int minute = 0) =>
            DateTime.SpecifyKind(DateTime.Today.AddDays(-days).AddHours(hour).AddMinutes(minute), DateTimeKind.Local).ToUniversalTime();

        public static DateTime PreviousDay(DayOfWeek day, int extraWeeks = 0)
        {
            int back = ((int)DateTime.Today.DayOfWeek - (int)day + 7) % 7;
            return DateTime.Today.AddDays(-(back == 0 ? 7 : back) - 7 * extraWeeks);
        }
    }
}
