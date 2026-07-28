using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Application.DTOs.AuditLogs.Stock;

namespace StockFlow.Application.Interfaces.ManagementApp.AuditLogs
{
    public interface IStockLogsService
    {
        Task<List<StockLogsDto>> GetAllStockLogsAsync(StockLogsPaginationDto pagination);
        Task CreateStockLogsAsync(CreateStockLogsDto log);
    }
}
