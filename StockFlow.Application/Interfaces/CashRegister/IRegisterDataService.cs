using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.CashRegister;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.CashRegister
{
    public interface IRegisterDataService
    {
        Task<string> FinalizePurchase(Purchase purchase, Guid employeeId);
        Task<ReturnResult> ProcessReturn(ReturnRequest request, Guid employeeId);
    }
}
