using StockFlow.ManagementApp.Services;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Ordering;

namespace StockFlow.ManagementApp.Interfaces.Ordering
{
    public interface IPurchaseOrderService
    {
        Task<ApiResult<List<PurchaseOrderDto>>> GetOrdersAsync(PurchaseOrderStatusDto? status, Guid? supplierId);
        Task<ApiResult<PurchaseOrderDto>> GetOrderAsync(int orderNumber);
        Task<ApiResult<List<ReorderSuggestionDto>>> GetSuggestionsAsync();
        Task<ApiResult<PurchaseOrderDto>> AddLineAsync(AddOrderLineRequest request);
        Task<ApiResult<PurchaseOrderDto>> UpdateLineAsync(int orderNumber, UpdateOrderLineRequest request);
        Task<ApiResult<PurchaseOrderDto>> CancelAsync(int orderNumber);
        Task<ApiResult<PurchaseOrderDto>> ReceiveAsync(int orderNumber, ReceiveOrderRequest request);
    }

    public interface ICustomerOrderService
    {
        Task<ApiResult<List<CustomerOrderDto>>> GetOrdersAsync(CustomerOrderStatusDto? status);
        Task<ApiResult<CustomerOrderDto>> CreateAsync(CreateCustomerOrderRequest request);
        Task<ApiResult<CustomerOrderDto>> CancelAsync(int orderNumber);
    }

    public interface IStockCountService
    {
        Task<ApiResult<List<StockCountDto>>> GetCountsAsync(StockCountStatusDto? status);
        Task<ApiResult<StockCountDto>> GetCountAsync(int countNumber);
        Task<ApiResult<StockCountDto>> CreateAsync(CreateStockCountRequest request);
        Task<ApiResult<StockCountDto>> CompleteAsync(int countNumber, CompleteStockCountRequest request);
        Task<ApiResult<StockCountDto>> CancelAsync(int countNumber);
    }
}
