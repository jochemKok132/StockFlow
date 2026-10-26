using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock
{
    public interface ISupplierRepository :
        IEfRepository<Supplier>,
        IEfUpdatableRepository<Supplier>,
        IEfCreatableRepository<Supplier>,
        IEfSoftDeletableRepository<Supplier>
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync(SupplierPaginationDto pagination);
        Task<Dictionary<Guid, int>> GetProductCountsAsync(IEnumerable<Guid> supplierIds);
    }
}
