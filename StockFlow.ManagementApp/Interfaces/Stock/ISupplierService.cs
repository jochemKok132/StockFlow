using StockFlow.ManagementApp.Services;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Supplier;

namespace StockFlow.ManagementApp.Interfaces.Stock
{
    public interface ISupplierService
    {
        Task<(List<SupplierDto>, string error)> GetPaginatedSuppliersAsync(SupplierPaginationDto pagination);
        Task<ApiResult<SupplierDto>> CreateSupplierAsync(CreateSupplierDto supplier);
        Task<ApiResult<SupplierDto>> UpdateSupplierAsync(UpdateSupplierDto supplier);
        Task<ApiResult<object>> SoftDeleteSupplierAsync(Guid id);
    }
}
