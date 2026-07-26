using StockFlow.Domain.Entities.IEntities;

namespace StockFlow.Application.Interfaces.Repositories.EfRepositories
{
    public interface IEfSoftDeletableRepository<T> where T : class, ISoftDeletable
    {
        Task SoftDeleteAsync(T entity);
    }
}
