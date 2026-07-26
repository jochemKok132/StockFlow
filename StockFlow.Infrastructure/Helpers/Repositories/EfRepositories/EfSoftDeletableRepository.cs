using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.IEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Infrastructure.Helpers.Repositories.EfRepositories
{
    public class EfSoftDeletableRepository<T> : IEfSoftDeletableRepository<T> where T : class, ISoftDeletable
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<T> _dbSet;
        public EfSoftDeletableRepository(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task SoftDeleteAsync(T entity)
        {
            entity.SoftDeletedAt = DateTime.UtcNow;
            entity.SoftDeleted = true;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
