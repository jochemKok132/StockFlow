using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.IEntities;
using StockFlow.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Infrastructure.Repositories.EfRepositories
{
    public class EfUpdatableRepository<T> : IEfUpdatableRepository<T> where T : class, IUpdatable
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<T> _dbSet;
        public EfUpdatableRepository(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
