using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.CashRegister;
using StockFlow.Application.Interfaces.CashRegister;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Services.ManagementApp.AuditLogs;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Services.CashRegister
{
    public class RegisterDataService(IProductService productService, ICashRegisterLogsService cashRegisterLogsService) : IRegisterDataService
    {
        public async Task FinalizePurchase(Purchase purchase, Guid employeeId)
        {
            if (purchase == null) throw new ArgumentNullException("Purchase cant be null.");
            
            foreach(var stock in purchase.ProductsSold)
            {
                await productService.UpdateProductStockAsync(stock.Product.Id, stock.Amount, employeeId);
            }
            await cashRegisterLogsService.CreateCashRegisterLogsAsync(purchase.ToCashRegisterLogsDto(employeeId));

        }
    }
}
