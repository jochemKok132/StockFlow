using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.Sales;

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

