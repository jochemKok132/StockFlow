using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock
{
    public interface IStockLogsRepository : 
        IEfRepository<StockLogs>, 
        IEfCreatableRepository<StockLogs>
    {
        Task<IEnumerable<StockLogs>> GetAllStockLogsAsync(StockLogsPaginationDto pagination);
        Task<IEnumerable<StockLogs>> GetLogsByIdentifierAsync(StockLogType stockLogType, string identifier);
    }
}
