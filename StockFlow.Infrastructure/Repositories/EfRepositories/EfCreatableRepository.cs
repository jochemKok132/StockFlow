using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.IEntities;
using StockFlow.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Infrastructure.Repositories.EfRepositories
{
    public class EfCreatableRepository<T> : IEfCreatableRepository<T> where T : class, ICreateable
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<T> _dbSet;
        public EfCreatableRepository(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
