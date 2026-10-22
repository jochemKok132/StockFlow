using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock;

namespace StockFlow.ManagementApp.Interfaces
{
    public interface IDeletedItemsService
    {
        Task<(List<DeletedItemDto>, string error)> GetAllAsync(DeletedItemTypeDto? type);
        Task<(bool restored, string message)> RestoreAsync(DeletedItemDto item);
    }
}
