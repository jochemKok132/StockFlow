using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;

namespace StockFlow.CashRegister.Interfaces
{
    public interface ICashRegisterService
    {
        Task<string> FinalizePurchaseAsync(Purchase purchase);
        Task<(ProductDto, string error)> GetProductAsync(string input);
        Task<(CustomerDto?, string error)> GetCustomerAsync(string customerId);
        Task<(CashRegisterLogsDto?, string error)> GetReceiptAsync(string receiptId);
        Task<(ReturnResult?, string error)> ProcessReturnAsync(ReturnRequest request);
    }
}

