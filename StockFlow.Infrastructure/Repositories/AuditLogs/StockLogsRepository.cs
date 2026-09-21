using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories.EfRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockFlow.Infrastructure.Repositories.Stock
{
    public class StockLogsRepository :
        EfRepository<StockLogs>,
        IEfCreatableRepository<StockLogs>,
        IStockLogsRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<StockLogs> _creatable;

        public StockLogsRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<StockLogs>(context);
        }

        public Task AddAsync(StockLogs entity) => _creatable.AddAsync(entity);

        public async Task<IEnumerable<StockLogs>> GetAllStockLogsAsync(StockLogsPaginationDto pagination)
        {
            var stockLogType = pagination.StockLogType?.ToDomain();
            var logType = pagination.LogType?.ToDomain();
            pagination.EmployeeName = pagination.EmployeeName?.ToLower();
            var query = _context.StockLogs
                .AsNoTracking()
                .Include(e => e.Employee)
                .Where(e =>
                    e.Employee.EmployeeId.Contains(pagination.EmployeeId ?? string.Empty) &&
                    e.Employee.FullName.ToLower().Contains(pagination.EmployeeName ?? string.Empty) &&
                    e.Identifier.Contains(pagination.Identifier ?? string.Empty) &&
                    (stockLogType == null || e.StockLogType == stockLogType) &&
                    (logType == null || e.LogType == logType));

            query = pagination.OrderType == OrderType.Descending
                ? pagination.OrderBy switch
                {
                    StockLogsOrderBy.EmployeeId => query.OrderByDescending(p => p.EmployeeId),
                    StockLogsOrderBy.StockLogType => query.OrderByDescending(p => p.StockLogType),
                    StockLogsOrderBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt),
                }
                : pagination.OrderBy switch
                {
                    StockLogsOrderBy.EmployeeId => query.OrderBy(p => p.EmployeeId),
                    StockLogsOrderBy.StockLogType => query.OrderBy(p => p.StockLogType),
                    StockLogsOrderBy.CreatedAt => query.OrderBy(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.CreatedAt),
                };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }
    }
}