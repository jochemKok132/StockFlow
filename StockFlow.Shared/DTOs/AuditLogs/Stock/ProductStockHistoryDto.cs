using StockFlow.Shared.DTOs.Stock.Product;

namespace StockFlow.Shared.DTOs.AuditLogs.Stock
{
    public class ProductStockHistoryDto
    {
        public required ProductDto Product { get; set; }
        public List<StockLogsDto> Logs { get; set; } = [];
    }
}
