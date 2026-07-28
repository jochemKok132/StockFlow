using StockFlow.Application.DTOs.AuditLogs.CashRegister;
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
    public class CashRegisterLogsService(ICashRegisterLogsRepository CashRegisterLogsRepository) : ICashRegisterLogsService
    {
        public async Task CreateCashRegisterLogsAsync(CreateCashRegisterLogsDto log)
        {
            if (log == null) throw new ArgumentNullException("CashRegisterLog cant be null.");

            await CashRegisterLogsRepository.AddAsync(log.ToCashRegisterLogsEntity());
        }

        public async Task<List<CashRegisterLogsDto>> GetAllCashRegisterLogsAsync(CashRegisterLogsPaginationDto pagination)
        {
            var CashRegisterLogss = await CashRegisterLogsRepository.GetAllCashRegisterLogsAsync(pagination);

            var result = new List<CashRegisterLogsDto>();
            foreach (var CashRegisterLogs in CashRegisterLogss)
            {
                result.Add(CashRegisterLogs.ToCashRegisterLogsDto());
            }
            return result;
        }

    }
}
