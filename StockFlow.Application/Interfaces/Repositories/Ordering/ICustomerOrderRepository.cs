using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Interfaces.Repositories.Ordering
{
    public interface ICustomerOrderRepository :
        IEfRepository<CustomerOrder>,
        IEfCreatableRepository<CustomerOrder>,
        IEfUpdatableRepository<CustomerOrder>
    {
        Task<int> GetNextOrderNumberAsync();
        Task<CustomerOrder?> GetByNumberAsync(int orderNumber);
        Task<List<CustomerOrder>> GetListAsync(CustomerOrderStatus? status);
        Task<List<CustomerOrder>> GetWaitingForProductsAsync(IEnumerable<Guid> productIds);
    }
}
