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

namespace StockFlow.Infrastructure.Repositories.AuditLogs
{
    public class EmployeeLogsRepository :
        EfRepository<EmployeeLogs>,
        IEfCreatableRepository<EmployeeLogs>,
        IEmployeeLogsRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<EmployeeLogs> _creatable;

        public EmployeeLogsRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<EmployeeLogs>(context);
        }

        public Task AddAsync(EmployeeLogs entity) => _creatable.AddAsync(entity);

        public async Task<IEnumerable<EmployeeLogs>> GetAllEmployeeLogsAsync(EmployeeLogsPaginationDto pagination)
        {
            var logType = pagination.LogType?.ToDomain();
            pagination.EmployeeName = pagination.EmployeeName?.ToLower();
            var query = _context.EmployeeLogs
                .AsNoTracking()
                .Include(e => e.Employee)
                .Where(e =>
                    e.Employee.EmployeeId.Contains(pagination.EmployeeId ?? string.Empty) &&
                    e.Employee.FullName.ToLower().Contains(pagination.EmployeeName ?? string.Empty) &&
                    e.Identifier.Contains(pagination.Identifier ?? string.Empty) &&
                    (logType == null || e.LogType == logType));

            query = pagination.OrderType == OrderType.Descending
                ? pagination.OrderBy switch
                {
                    EmployeeLogsOrderBy.EmployeeId => query.OrderByDescending(p => p.EmployeeId),
                    EmployeeLogsOrderBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt),
                }
                : pagination.OrderBy switch
                {
                    EmployeeLogsOrderBy.EmployeeId => query.OrderBy(p => p.EmployeeId),
                    EmployeeLogsOrderBy.CreatedAt => query.OrderBy(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.CreatedAt),
                };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }
    }
}