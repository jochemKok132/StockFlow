using StockFlow.ManagementApp.Interfaces;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock;

namespace StockFlow.ManagementApp.Services
{
    public class DeletedItemsService(IHttpService httpService) : IDeletedItemsService
    {
        public async Task<(List<DeletedItemDto>, string error)> GetAllAsync(DeletedItemTypeDto? type)
        {
            var response = await httpService.GetAsync<List<DeletedItemDto>>("DeletedItems/GetAll" + (type is null ? "" : $"?type={type}"));
            return response.Succeeded ? (response.Value ?? [], "") : ([], response.Message ?? "The deleted items could not be loaded.");
        }

        public async Task<(bool restored, string message)> RestoreAsync(DeletedItemDto item)
        {
            var response = await httpService.PostAsync<DeletedItemDto, object>($"DeletedItems/Restore/{item.Type}/{item.Id}", new { });
            return (response.Succeeded, response.Message ?? (response.Succeeded ? "Restored." : "The item could not be restored."));
        }
    }
}
