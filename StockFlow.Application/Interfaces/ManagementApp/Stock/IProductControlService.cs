using StockFlow.Shared.DTOs.Stock.Product;

namespace StockFlow.Application.Interfaces.ManagementApp.Stock
{
    public interface IProductControlService
    {
        Task<ProductDto> SchedulePriceAsync(SchedulePriceRequest request, Guid employeeId);
        Task<ProductDto> CancelScheduledPriceAsync(Guid productId, Guid priceId, Guid employeeId);
        Task<ProductDto> RecallAsync(RecallRequest request, Guid employeeId);
        Task<ProductDto> UndoRecallAsync(Guid productId, Guid employeeId);
        Task<ProductDto> BlockOrdersAsync(OrderBlockRequest request, Guid employeeId);
        Task<ProductDto> UnblockOrdersAsync(Guid productId, Guid employeeId);
        Task<int> ApplyDuePricesAsync(DateTime today);
    }
}
