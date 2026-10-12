using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Application.Validations;
using StockFlow.Application.Validations.AuditLogs;
using StockFlow.Application.Validations.CashRegister;

namespace StockFlow.Application.Services.ManagementApp.AuditLogs
{
    public class CashRegisterLogsService(ICashRegisterLogsRepository cashRegisterLogsRepository) : ICashRegisterLogsService
    {
        public async Task<string> CreateCashRegisterLogsAsync(CreateCashRegisterLogsDto log)
        {
            log.Validate();

            string receiptId = await cashRegisterLogsRepository.GetNewReceiptIdAsync();
            await cashRegisterLogsRepository.AddAsync(log.ToCashRegisterLogsEntity(receiptId));
            return receiptId;
        }

        public async Task<CashRegisterLogsDto> GetByReceiptIdAsync(string receiptId)
        {
            receiptId.ValidateReceiptId();

            var log = await cashRegisterLogsRepository.GetByReceiptIdAsync(receiptId);
            if (log is null) throw new KeyNotFoundException($"Receipt {receiptId} not found.");

            return log.ToCashRegisterLogsDto();
        }

        public async Task<List<CashRegisterLogsDto>> GetAllCashRegisterLogsAsync(CashRegisterLogsPaginationDto pagination)
        {
            pagination.Validate();

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
