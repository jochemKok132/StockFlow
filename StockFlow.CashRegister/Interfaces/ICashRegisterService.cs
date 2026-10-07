using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.CashRegister;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;

namespace StockFlow.CashRegister.Interfaces
{
    public interface ICashRegisterService
    {
        Task<string> FinalizePurchaseAsync(Purchase purchase);
        Task<(ProductDto, string error)> GetProductAsync(string input);
    }
}

