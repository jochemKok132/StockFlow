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
    public class CustomerOrderController(ICustomerOrderService customerOrderService) : ControllerBase
    {
        [Authorize(Roles = Permissions.ViewCustomerOrders)]
        [HttpGet("GetAll")]
        public Task<IActionResult> GetAllAsync([FromQuery] CustomerOrderStatusDto? status) =>
            this.RunAsync(() => customerOrderService.GetOrdersAsync(status));

        [Authorize(Roles = Permissions.ViewCustomerOrders)]
        [HttpGet("{orderNumber:int}")]
        public Task<IActionResult> GetAsync(int orderNumber) =>
            this.RunAsync(() => customerOrderService.GetByNumberAsync(orderNumber));

        [Authorize(Roles = Permissions.ManageCustomerOrders)]
        [HttpPost("Create")]
        public Task<IActionResult> CreateAsync(CreateCustomerOrderRequest request) =>
            this.RunAsEmployeeAsync(userId => customerOrderService.CreateAsync(request, userId),
                o => o.Status == CustomerOrderStatusDto.ReadyForPickup
                    ? $"Customer order {o.OrderNumber} created and ready for pickup."
                    : $"Customer order {o.OrderNumber} created. Missing products are added to the next supplier order.");

        [Authorize(Roles = Permissions.ManageCustomerOrders)]
        [HttpPost("{orderNumber:int}/Cancel")]
        public Task<IActionResult> CancelAsync(int orderNumber) =>
            this.RunAsync(() => customerOrderService.CancelAsync(orderNumber), o => $"Customer order {o.OrderNumber} cancelled.");
    }
}
