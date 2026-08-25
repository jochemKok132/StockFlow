using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories.EfRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockFlow.Infrastructure.Repositories.Stock
{
    public class CustomerLogsRepository :
        EfRepository<CustomerLogs>,
        IEfCreatableRepository<CustomerLogs>,
        ICustomerLogsRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<CustomerLogs> _creatable;

        public CustomerLogsRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<CustomerLogs>(context);
        }

        public Task AddAsync(CustomerLogs entity) => _creatable.AddAsync(entity);


        public async Task<IEnumerable<CustomerLogs>> GetAllCustomerLogsAsync(CustomerLogsPaginationDto pagination)
        {
            pagination.CustomerName = pagination.CustomerName?.ToLower();

            var query = _context.CustomerLogs
                .AsNoTracking()
                .Where(e =>
                    e.Customer.FullName.Contains(pagination.CustomerName ?? string.Empty) &&
                    e.Customer.Email.Contains(pagination.Email ?? string.Empty) &&
                    (pagination.LogType == null || e.LogType.ToDto() == pagination.LogType));

            query = pagination.OrderType == OrderType.Descending
                ? pagination.OrderBy switch
                {
                    CustomerLogsOrderBy.CustomerName => query.OrderByDescending(p => p.Customer.FullName),
                    CustomerLogsOrderBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt),
                }
                : pagination.OrderBy switch
                {
                    CustomerLogsOrderBy.CustomerName => query.OrderBy(p => p.Customer.FullName),
                    CustomerLogsOrderBy.CreatedAt => query.OrderBy(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.CreatedAt),
                };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }
    }
}