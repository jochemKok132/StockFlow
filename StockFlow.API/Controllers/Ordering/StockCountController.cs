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
    public class StockCountController(IStockCountService stockCountService) : ControllerBase
    {
        [Authorize(Roles = Permissions.ViewStockCounts)]
        [HttpGet("GetAll")]
        public Task<IActionResult> GetAllAsync([FromQuery] StockCountStatusDto? status) =>
            this.RunAsync(() => stockCountService.GetCountsAsync(status));

        [Authorize(Roles = Permissions.ViewStockCounts)]
        [HttpGet("{countNumber:int}")]
        public Task<IActionResult> GetAsync(int countNumber) =>
            this.RunAsync(() => stockCountService.GetByNumberAsync(countNumber));

        [Authorize(Roles = Permissions.PlanStockCounts)]
        [HttpPost("Create")]
        public Task<IActionResult> CreateAsync(CreateStockCountRequest request) =>
            this.RunAsEmployeeAsync(userId => stockCountService.CreateAsync(request, userId),
                c => $"Stock count {c.CountNumber} created for shelf {c.ShelfName}.");

        [Authorize(Roles = Permissions.CountStock)]
        [HttpPost("{countNumber:int}/Complete")]
        public Task<IActionResult> CompleteAsync(int countNumber, CompleteStockCountRequest request) =>
            this.RunAsEmployeeAsync(userId => stockCountService.CompleteAsync(countNumber, request, userId),
                c => $"Stock count {c.CountNumber} completed.");

        [Authorize(Roles = Permissions.PlanStockCounts)]
        [HttpPost("{countNumber:int}/Cancel")]
        public Task<IActionResult> CancelAsync(int countNumber) =>
            this.RunAsync(() => stockCountService.CancelAsync(countNumber), c => $"Stock count {c.CountNumber} cancelled.");
    }
}
