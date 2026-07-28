using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Services.ManagementApp.AuditLogs
{
    public class StockLogsService(IStockLogsRepository stockLogsRepository) : IStockLogsService
    {
        public async Task CreateStockLogsAsync(CreateStockLogsDto log)
        {
            if (log == null) throw new ArgumentNullException("StockLog cant be null.");

            await stockLogsRepository.AddAsync(log.ToStockLogsEntity());
        }

        public async Task<List<StockLogsDto>> GetAllStockLogsAsync(StockLogsPaginationDto pagination)
        {
            var stockLogs = await stockLogsRepository.GetAllStockLogsAsync(pagination);

            var result = new List<StockLogsDto>();
            foreach (var stockLog in stockLogs)
            {
                result.Add(stockLog.ToStockLogsDto());
            }
            return result;
        }

    }
}
