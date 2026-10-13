namespace StockFlow.Shared.DTOs.CashRegister
{
    public class ReturnRequest
    {
        public string ReceiptId { get; set; } = string.Empty;
        public List<ReturnItem> Items { get; set; } = [];
    }

    public class ReturnItem
    {
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
    }

    public class ReturnResult
    {
        public string ReceiptId { get; set; } = string.Empty;
        public int ItemsReturned { get; set; }
        public decimal RefundAmount { get; set; }
        public int PointsDeducted { get; set; }
    }
}
