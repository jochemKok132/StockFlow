using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.Ordering;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Ordering;

namespace StockFlow.ManagementApp.Services.Ordering
{
    public class PurchaseOrderService(IHttpService httpService) : IPurchaseOrderService
    {
        public async Task<ApiResult<List<PurchaseOrderDto>>> GetOrdersAsync(PurchaseOrderStatusDto? status, Guid? supplierId)
        {
            var query = new List<string>();
            if (status is not null) query.Add($"status={status}");
            if (supplierId is not null) query.Add($"supplierId={supplierId}");
            string url = "Order/GetAll" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
            return ApiResult<List<PurchaseOrderDto>>.From(await httpService.GetAsync<List<PurchaseOrderDto>>(url), "The orders could not be loaded.");
        }

        public async Task<ApiResult<PurchaseOrderDto>> GetOrderAsync(int orderNumber) =>
            ApiResult<PurchaseOrderDto>.From(await httpService.GetAsync<PurchaseOrderDto>($"Order/{orderNumber}"), "The order could not be loaded.");

        public async Task<ApiResult<List<ReorderSuggestionDto>>> GetSuggestionsAsync() =>
            ApiResult<List<ReorderSuggestionDto>>.From(await httpService.GetAsync<List<ReorderSuggestionDto>>("Order/Suggestions"), "The suggestions could not be loaded.");

        public async Task<ApiResult<PurchaseOrderDto>> AddLineAsync(AddOrderLineRequest request) =>
            ApiResult<PurchaseOrderDto>.From(await httpService.PostAsync<PurchaseOrderDto, AddOrderLineRequest>("Order/AddLine", request), "The product could not be ordered.");

        public async Task<ApiResult<PurchaseOrderDto>> UpdateLineAsync(int orderNumber, UpdateOrderLineRequest request) =>
            ApiResult<PurchaseOrderDto>.From(await httpService.PutAsync<PurchaseOrderDto, UpdateOrderLineRequest>($"Order/{orderNumber}/Line", request), "The order could not be changed.");

        public async Task<ApiResult<PurchaseOrderDto>> CancelAsync(int orderNumber) =>
            ApiResult<PurchaseOrderDto>.From(await httpService.PostAsync<PurchaseOrderDto, object>($"Order/{orderNumber}/Cancel", new { }), "The order could not be cancelled.");

        public async Task<ApiResult<PurchaseOrderDto>> ReceiveAsync(int orderNumber, ReceiveOrderRequest request) =>
            ApiResult<PurchaseOrderDto>.From(await httpService.PostAsync<PurchaseOrderDto, ReceiveOrderRequest>($"Order/{orderNumber}/Receive", request), "The delivery could not be saved.");
    }

    public class CustomerOrderService(IHttpService httpService) : ICustomerOrderService
    {
        public async Task<ApiResult<List<CustomerOrderDto>>> GetOrdersAsync(CustomerOrderStatusDto? status) =>
            ApiResult<List<CustomerOrderDto>>.From(await httpService.GetAsync<List<CustomerOrderDto>>("CustomerOrder/GetAll" + (status is null ? "" : $"?status={status}")), "The customer orders could not be loaded.");

        public async Task<ApiResult<CustomerOrderDto>> CreateAsync(CreateCustomerOrderRequest request) =>
            ApiResult<CustomerOrderDto>.From(await httpService.PostAsync<CustomerOrderDto, CreateCustomerOrderRequest>("CustomerOrder/Create", request), "The customer order could not be created.");

        public async Task<ApiResult<CustomerOrderDto>> CancelAsync(int orderNumber) =>
            ApiResult<CustomerOrderDto>.From(await httpService.PostAsync<CustomerOrderDto, object>($"CustomerOrder/{orderNumber}/Cancel", new { }), "The customer order could not be cancelled.");
    }

    public class StockCountService(IHttpService httpService) : IStockCountService
    {
        public async Task<ApiResult<List<StockCountDto>>> GetCountsAsync(StockCountStatusDto? status) =>
            ApiResult<List<StockCountDto>>.From(await httpService.GetAsync<List<StockCountDto>>("StockCount/GetAll" + (status is null ? "" : $"?status={status}")), "The stock counts could not be loaded.");

        public async Task<ApiResult<StockCountDto>> GetCountAsync(int countNumber) =>
            ApiResult<StockCountDto>.From(await httpService.GetAsync<StockCountDto>($"StockCount/{countNumber}"), "The stock count could not be loaded.");

        public async Task<ApiResult<StockCountDto>> CreateAsync(CreateStockCountRequest request) =>
            ApiResult<StockCountDto>.From(await httpService.PostAsync<StockCountDto, CreateStockCountRequest>("StockCount/Create", request), "The stock count could not be created.");

        public async Task<ApiResult<StockCountDto>> CompleteAsync(int countNumber, CompleteStockCountRequest request) =>
            ApiResult<StockCountDto>.From(await httpService.PostAsync<StockCountDto, CompleteStockCountRequest>($"StockCount/{countNumber}/Complete", request), "The stock count could not be completed.");

        public async Task<ApiResult<StockCountDto>> CancelAsync(int countNumber) =>
            ApiResult<StockCountDto>.From(await httpService.PostAsync<StockCountDto, object>($"StockCount/{countNumber}/Cancel", new { }), "The stock count could not be cancelled.");
    }
}
