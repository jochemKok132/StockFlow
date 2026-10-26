using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Interfaces.Repositories.Ordering
{
    public interface IPurchaseOrderRepository :
        IEfRepository<PurchaseOrder>,
        IEfCreatableRepository<PurchaseOrder>,
        IEfUpdatableRepository<PurchaseOrder>
    {
        Task<int> GetNextOrderNumberAsync();
        Task<PurchaseOrder?> GetByNumberAsync(int orderNumber);
        Task<PurchaseOrder?> GetOpenForSupplierAsync(Guid supplierId);
        Task<List<PurchaseOrder>> GetOpenOrdersForSupplierAsync(Guid supplierId);
        Task<List<PurchaseOrder>> GetListAsync(PurchaseOrderStatus? status, Guid? supplierId);
        Task<List<PurchaseOrder>> GetOpenOrdersDueAsync(DateTime today);
        Task<List<PurchaseOrderLine>> GetOpenLinesForCustomerOrderAsync(Guid customerOrderId);
        Task<List<PurchaseOrderLine>> GetOpenLinesForProductAsync(Guid productId);
        Task<Dictionary<Guid, int>> GetCustomerOrderNumbersAsync(IEnumerable<Guid> customerOrderIds);
        void RemoveLine(PurchaseOrderLine line);
    }
}
