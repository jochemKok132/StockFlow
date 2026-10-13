using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Stock.Shelf;

namespace StockFlow.ManagementApp.Interfaces.Stock
{
    public interface IShelfService
    {
        Task<(List<ShelfDto>, string error)> GetPaginatedShelfAsync(ShelfPaginationDto paginationDto);
        Task<string> CreateShelfAsync(CreateShelfDto product);
        Task<string> UpdateShelfAsync(UpdateShelfDto product);
        Task<string> SoftDeleteShelfAsync(Guid id);
    }
}

