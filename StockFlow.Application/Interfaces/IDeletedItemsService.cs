using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock;

namespace StockFlow.Application.Interfaces
{
    public interface IDeletedItemsService
    {
        Task<List<DeletedItemDto>> GetDeletedItemsAsync(DeletedItemTypeDto? type);
        Task<DeletedItemDto> RestoreAsync(DeletedItemTypeDto type, Guid id, Guid employeeId);
    }
}
