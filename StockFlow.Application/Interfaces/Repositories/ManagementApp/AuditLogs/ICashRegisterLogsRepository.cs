using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock
{
    public interface ICashRegisterLogsRepository : 
        IEfRepository<CashRegisterLogs>, 
        IEfCreatableRepository<CashRegisterLogs>
    {
        Task<IEnumerable<CashRegisterLogs>> GetAllCashRegisterLogsAsync(CashRegisterLogsPaginationDto pagination);
    }
}
