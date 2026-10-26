using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Supplier;

namespace StockFlow.Application.Interfaces.ManagementApp.Stock
{
    public interface ISupplierService
    {
        Task<List<SupplierDto>> GetAllSuppliersAsync(SupplierPaginationDto pagination);
        Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto dto);
        Task<SupplierDto> UpdateSupplierAsync(UpdateSupplierDto dto);
        Task SoftDeleteSupplierAsync(Guid id);
    }
}
