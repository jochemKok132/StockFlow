using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Ordering;

namespace StockFlow.Application.Interfaces.Ordering
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderDto>> GetOrdersAsync(PurchaseOrderStatusDto? status, Guid? supplierId);
        Task<PurchaseOrderDto> GetByNumberAsync(int orderNumber);
        Task<PurchaseOrderDto> AddLineAsync(AddOrderLineRequest request, Guid employeeId);
        Task<PurchaseOrderDto> UpdateLineAsync(int orderNumber, UpdateOrderLineRequest request);
        Task<PurchaseOrderDto> CancelAsync(int orderNumber);
        Task<PurchaseOrderDto> ReceiveAsync(int orderNumber, ReceiveOrderRequest request, Guid employeeId);
        Task<List<ReorderSuggestionDto>> GetReorderSuggestionsAsync();
        Task<PurchaseOrder> AddToOpenOrderAsync(Product product, int amount, OrderLineSource source, Guid? customerOrderId, Guid? employeeId, DateTime today);
        Task RemoveOpenCustomerOrderLinesAsync(Guid customerOrderId);
        Task<int> CreateAutomaticOrdersAsync(DateTime today);
        Task<int> SendDueOrdersAsync(DateTime today);
        Task RescheduleOpenOrdersAsync(Guid supplierId, DayOfWeek orderDay, DateTime today);
    }
}
