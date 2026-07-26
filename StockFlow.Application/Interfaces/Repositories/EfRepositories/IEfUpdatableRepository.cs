using StockFlow.Domain.Entities.IEntities;

namespace StockFlow.Application.Interfaces.Repositories.EfRepositories
{
    public interface IEfUpdatableRepository<T> where T : class, IUpdatable
    {
        Task UpdateAsync(T entity);
    }
}
