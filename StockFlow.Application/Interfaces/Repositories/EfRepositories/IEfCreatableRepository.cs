using StockFlow.Domain.Entities.IEntities;

namespace StockFlow.Application.Interfaces.Repositories.EfRepositories
{
    public interface IEfCreatableRepository<T> where T : class, ICreateable
    {
        Task AddAsync(T entity);
    }
}
