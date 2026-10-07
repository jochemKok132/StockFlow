using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.CashRegister;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.CashRegister
{
    public interface IRegisterDataService
    {
        Task FinalizePurchase(Purchase purchase, Guid employeeId);
    }
}
