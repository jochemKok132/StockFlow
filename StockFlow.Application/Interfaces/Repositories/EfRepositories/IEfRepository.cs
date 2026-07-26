using StockFlow.Domain.Entities.IEntities;

namespace StockFlow.Application.Interfaces.Repositories.EfRepositories
{
    public interface IEfRepository<T> where T : class, IEntity
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
    }
}
