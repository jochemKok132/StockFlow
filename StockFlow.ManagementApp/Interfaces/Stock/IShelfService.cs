using StockFlow.Application.DTOs.AuditLogs.CashRegister;
using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.Shelf;

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

