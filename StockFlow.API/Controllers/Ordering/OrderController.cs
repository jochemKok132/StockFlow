using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Interfaces.Ordering;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Ordering;

namespace StockFlow.API.Controllers.Ordering
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController(IPurchaseOrderService purchaseOrderService) : ControllerBase
    {
        [Authorize(Roles = Permissions.ViewOrders)]
        [HttpGet("GetAll")]
        public Task<IActionResult> GetAllAsync([FromQuery] PurchaseOrderStatusDto? status, [FromQuery] Guid? supplierId) =>
            this.RunAsync(() => purchaseOrderService.GetOrdersAsync(status, supplierId));

        [Authorize(Roles = Permissions.ViewOrders)]
        [HttpGet("{orderNumber:int}")]
        public Task<IActionResult> GetAsync(int orderNumber) =>
            this.RunAsync(() => purchaseOrderService.GetByNumberAsync(orderNumber));

        [Authorize(Roles = Permissions.ViewOrders)]
        [HttpGet("Suggestions")]
        public Task<IActionResult> GetSuggestionsAsync() =>
            this.RunAsync(() => purchaseOrderService.GetReorderSuggestionsAsync());

        [Authorize(Roles = Permissions.ManageOrders)]
        [HttpPost("AddLine")]
        public Task<IActionResult> AddLineAsync(AddOrderLineRequest request) =>
            this.RunAsEmployeeAsync(userId => purchaseOrderService.AddLineAsync(request, userId),
                o => $"Added to order {o.OrderNumber} for {o.SupplierName} (order day {o.OrderDate:dd-MM-yyyy}).");

        [Authorize(Roles = Permissions.ManageOrders)]
        [HttpPut("{orderNumber:int}/Line")]
        public Task<IActionResult> UpdateLineAsync(int orderNumber, UpdateOrderLineRequest request) =>
            this.RunAsync(() => purchaseOrderService.UpdateLineAsync(orderNumber, request), _ => "Order updated.");

        [Authorize(Roles = Permissions.ManageOrders)]
        [HttpPost("{orderNumber:int}/Cancel")]
        public Task<IActionResult> CancelAsync(int orderNumber) =>
            this.RunAsync(() => purchaseOrderService.CancelAsync(orderNumber), o => $"Order {o.OrderNumber} cancelled.");

        [Authorize(Roles = Permissions.ReceiveOrders)]
        [HttpPost("{orderNumber:int}/Receive")]
        public Task<IActionResult> ReceiveAsync(int orderNumber, ReceiveOrderRequest request) =>
            this.RunAsEmployeeAsync(userId => purchaseOrderService.ReceiveAsync(orderNumber, request, userId),
                o => $"Order {o.OrderNumber} received: {o.Lines.Sum(l => l.ReceivedAmount)} item(s) added to stock.");
    }
}
