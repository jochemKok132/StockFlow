using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Shared.DTOs.AuditLogs.Stock;

namespace StockFlow.Application.Interfaces.ManagementApp.AuditLogs
{
    public interface IStockLogsService
    {
        Task<List<StockLogsDto>> GetAllStockLogsAsync(StockLogsPaginationDto pagination);
        Task CreateStockLogsAsync(CreateStockLogsDto log);
        Task<ProductStockHistoryDto> GetProductStockHistoryAsync(int barcode, ProductHistoryPaginationDto pagination);
    }
}
