using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Application.Interfaces.Repositories.ManagementApp;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories.EfRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Infrastructure.Repositories
{
    public class CustomerRepository :
        EfRepository<Customer>,
        IEfUpdatableRepository<Customer>,
        IEfCreatableRepository<Customer>,
        IEfSoftDeletableRepository<Customer>,
        ICustomerRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<Customer> _creatable;
        private readonly EfUpdatableRepository<Customer> _updatable;
        private readonly EfSoftDeletableRepository<Customer> _softDeletable;

        public CustomerRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<Customer>(context);
            _updatable = new EfUpdatableRepository<Customer>(context);
            _softDeletable = new EfSoftDeletableRepository<Customer>(context);
        }

        public Task AddAsync(Customer entity) => _creatable.AddAsync(entity);
        public Task UpdateAsync(Customer entity) => _updatable.UpdateAsync(entity);
        public Task SoftDeleteAsync(Customer entity) => _softDeletable.SoftDeleteAsync(entity);

        public Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return _context.Customers.FirstOrDefaultAsync(e => e.Email == email);
        }
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync(CustomerPaginationDto pagination)
        {
            var query = _context.Customers
                .AsNoTracking()
                .Where(e =>
                !e.SoftDeleted &&
                e.FullName.ToLower().Contains((pagination.CustomerName ?? string.Empty).ToLower()) &&
                e.HouseNumber.ToLower().Contains((pagination.HouseNumber ?? string.Empty).ToLower()) &&
                e.PostalCode.ToLower().Contains((pagination.PostalCode ?? string.Empty).ToLower()) &&
                e.Email.ToLower().Contains((pagination.Email ?? string.Empty).ToLower()));

            query = pagination.OrderType == OrderType.Descending
                ? pagination.OrderBy switch
                {
                    CustomerOrderBy.FullName => query.OrderByDescending(p => p.FullName),
                    CustomerOrderBy.SavedPoints => query.OrderByDescending(p => p.SavedPoints),
                    CustomerOrderBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
                    CustomerOrderBy.UpdatedAt => query.OrderByDescending(p => p.UpdatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt),
                }
                : pagination.OrderBy switch
                {
                    CustomerOrderBy.FullName => query.OrderBy(p => p.FullName),
                    CustomerOrderBy.SavedPoints => query.OrderBy(p => p.SavedPoints),
                    CustomerOrderBy.CreatedAt => query.OrderBy(p => p.CreatedAt),
                    CustomerOrderBy.UpdatedAt => query.OrderBy(p => p.UpdatedAt),
                    _ => query.OrderBy(p => p.CreatedAt),
                };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }
    }
}
