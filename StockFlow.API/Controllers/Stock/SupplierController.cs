using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Supplier;

namespace StockFlow.API.Controllers.Stock
{
    [ApiController]
    [Route("[controller]")]
    public class SupplierController(ISupplierService supplierService) : ControllerBase
    {
        [Authorize(Roles = Permissions.ViewSuppliers)]
        [HttpGet("GetAll")]
        public Task<IActionResult> GetAllAsync([FromQuery] SupplierPaginationDto pagination) =>
            this.RunAsync(() => supplierService.GetAllSuppliersAsync(pagination));

        [Authorize(Roles = Permissions.ManageSuppliers)]
        [HttpPost("Create")]
        public Task<IActionResult> CreateAsync(CreateSupplierDto request) =>
            this.RunAsync(() => supplierService.CreateSupplierAsync(request), s => $"Supplier {s.SupplierName} created.");

        [Authorize(Roles = Permissions.ManageSuppliers)]
        [HttpPut("Update")]
        public Task<IActionResult> UpdateAsync(UpdateSupplierDto request) =>
            this.RunAsync(() => supplierService.UpdateSupplierAsync(request), s => $"Supplier {s.SupplierName} updated.");

        [Authorize(Roles = Permissions.ManageSuppliers)]
        [HttpDelete("SoftDelete/{id:guid}")]
        public Task<IActionResult> SoftDeleteAsync(Guid id) =>
            this.RunAsync(() => supplierService.SoftDeleteSupplierAsync(id), "Supplier deleted.");
    }
}
