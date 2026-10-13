using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Stock.Sales;

namespace StockFlow.ManagementApp.Interfaces.Stock
{
    public interface ISalesService
    {
        Task<(List<SalesDto>, string error)> GetPaginatedSalesAsync(SalesPaginationDto paginationDto);
        Task<string> CreateSalesAsync(CreateSalesDto product);
        Task<string> UpdateSalesAsync(UpdateSalesDto product);
        Task<string> SoftDeleteSalesAsync(Guid id);
    }
}

