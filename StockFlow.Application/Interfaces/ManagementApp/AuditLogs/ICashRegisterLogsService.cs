using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Shared.DTOs.AuditLogs.CashRegister;

namespace StockFlow.Application.Interfaces.ManagementApp.AuditLogs
{
    public interface ICashRegisterLogsService
    {
        Task<List<CashRegisterLogsDto>> GetAllCashRegisterLogsAsync(CashRegisterLogsPaginationDto pagination);
        Task<string> CreateCashRegisterLogsAsync(CreateCashRegisterLogsDto log);
        Task<CashRegisterLogsDto> GetByReceiptIdAsync(string receiptId);
    }
}
