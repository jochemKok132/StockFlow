using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.CashRegister
{
    public interface IRegisterDataService
    {
        Task<string> FinalizePurchase(Purchase purchase, Guid employeeId);
        Task<ReturnResult> ProcessReturn(ReturnRequest request, Guid employeeId);
        Task<CustomerDto> GetCustomerAsync(string customerId);
        Task<List<CustomerDto>> FindCustomersByAddressAsync(string postalCode, string houseNumber);
    }
}
