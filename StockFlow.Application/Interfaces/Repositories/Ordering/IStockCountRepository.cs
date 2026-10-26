using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Interfaces.Repositories.Ordering
{
    public interface IStockCountRepository :
        IEfRepository<StockCount>,
        IEfCreatableRepository<StockCount>,
        IEfUpdatableRepository<StockCount>
    {
        Task<int> GetNextCountNumberAsync();
        Task<StockCount?> GetByNumberAsync(int countNumber);
        Task<List<StockCount>> GetListAsync(StockCountStatus? status);
        Task<StockCount?> GetOpenCountForShelfAsync(Guid shelfId, StockCountType type);
    }
}
