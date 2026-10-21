namespace StockFlow.Shared.DTOs.Statistics
{
    public class StatisticsDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Purchases { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageReceipt { get; set; }
        public int PurchasesWithCustomer { get; set; }
        public double CustomerCardPercentage { get; set; }
        public int ItemsSold { get; set; }
        public int ItemsReturned { get; set; }
        public decimal Discounts { get; set; }
        public int PointsUsed { get; set; }
        public int PointsEarned { get; set; }
        public int NewCustomers { get; set; }
        public List<DailyStatisticsDto> Days { get; set; } = [];
        public List<TopProductDto> TopProducts { get; set; } = [];
    }

    public class DailyStatisticsDto
    {
        public DateTime Date { get; set; }
        public int Purchases { get; set; }
        public decimal Revenue { get; set; }
        public double CustomerCardPercentage { get; set; }
    }

    public class TopProductDto
    {
        public int Barcode { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int ItemsSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
