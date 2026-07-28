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
    public class CashRegisterLogsService(ICashRegisterLogsRepository cashRegisterLogsRepository) : ICashRegisterLogsService
    {
        public async Task CreateCashRegisterLogsAsync(CreateCashRegisterLogsDto log)
        {
            if (log == null) throw new ArgumentNullException("CashRegisterLog cant be null.");

            await cashRegisterLogsRepository.AddAsync(log.ToCashRegisterLogsEntity());
        }

        public async Task<List<CashRegisterLogsDto>> GetAllCashRegisterLogsAsync(CashRegisterLogsPaginationDto pagination)
        {
            var cashRegisterLogs = await cashRegisterLogsRepository.GetAllCashRegisterLogsAsync(pagination);

            var result = new List<CashRegisterLogsDto>();
            foreach (var cashRegisterLog in cashRegisterLogs)
            {
                result.Add(cashRegisterLog.ToCashRegisterLogsDto());
            }
            return result;
        }

    }
}
