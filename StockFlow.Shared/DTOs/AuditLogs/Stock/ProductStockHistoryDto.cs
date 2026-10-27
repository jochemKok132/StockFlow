using StockFlow.Shared.DTOs.Stock.Product;

namespace StockFlow.Shared.DTOs.AuditLogs.Stock
{
    public class ProductStockHistoryDto
    {
        public required ProductDto Product { get; set; }
        public List<StockLogsDto> Logs { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int MatchingLogs { get; set; }
        public int TotalLogs { get; set; }
        public int UnitsSold { get; set; }
        public int UnitsReturned { get; set; }
        public int UnitsWrittenOff { get; set; }
        public int UnitsAdded { get; set; }
    }
}
