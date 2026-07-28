using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Application.DTOs.AuditLogs.CashRegister;

namespace StockFlow.Application.Interfaces.ManagementApp.AuditLogs
{
    public interface ICashRegisterLogsService
    {
        Task<List<CashRegisterLogsDto>> GetAllCashRegisterLogsAsync(CashRegisterLogsPaginationDto pagination);
        Task CreateCashRegisterLogsAsync(CreateCashRegisterLogsDto log);
    }
}
