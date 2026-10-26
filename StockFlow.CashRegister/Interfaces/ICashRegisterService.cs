using StockFlow.Shared.DTOs.Ordering;
using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Releases;
using StockFlow.Shared.DTOs.Stock.Product;

namespace StockFlow.CashRegister.Interfaces
{
    public interface ICashRegisterService
    {
        Task<string> FinalizePurchaseAsync(Purchase purchase);
        Task<(ProductDto, string error)> GetProductAsync(string input);
        Task<(CustomerDto?, string error)> GetCustomerAsync(string customerId);
        Task<(List<CustomerDto>, string error)> FindCustomersByAddressAsync(string postalCode, string houseNumber);
        Task<(CashRegisterLogsDto?, string error)> GetReceiptAsync(string receiptId);
        Task<(ReturnResult?, string error)> ProcessReturnAsync(ReturnRequest request);
        Task<ReleaseDto?> GetLatestReleaseAsync();
        Task<(CustomerOrderDto?, string error)> GetCustomerOrderAsync(string orderNumber);
    }
}

