using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities.IEntities;
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
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }
        public Task SaveChangesAsync() => _context.SaveChangesAsync();

    }
}
