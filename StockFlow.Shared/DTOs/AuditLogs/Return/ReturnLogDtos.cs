namespace StockFlow.Shared.DTOs.AuditLogs.Return
{
    public class ReturnLogDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ReceiptId { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public decimal RefundAmount { get; set; }
        public int PointsDeducted { get; set; }
        public int ItemsReturned { get; set; }
        public List<ReturnLogLineDto> Lines { get; set; } = [];
    }

    public class ReturnLogLineDto
    {
        public int Barcode { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Amount { get; set; }
        public decimal PriceAtTimeOfSale { get; set; }
        public decimal RefundAmount { get; set; }
    }
}
