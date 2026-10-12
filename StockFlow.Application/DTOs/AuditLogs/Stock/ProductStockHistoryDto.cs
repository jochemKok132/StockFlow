using StockFlow.Application.DTOs.Stock.Product;

namespace StockFlow.Application.DTOs.AuditLogs.Stock
{
    public class ProductStockHistoryDto
    {
        public required ProductDto Product { get; set; }
        public List<StockLogsDto> Logs { get; set; } = [];
    }
}
