using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.AuditLogs;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories.EfRepositories;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.Infrastructure.Repositories.AuditLogs
{
    public class ReturnLogsRepository :
        EfRepository<ReturnLogs>,
        IEfCreatableRepository<ReturnLogs>,
        IReturnLogsRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<ReturnLogs> _creatable;

        public ReturnLogsRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<ReturnLogs>(context);
        }

        public Task AddAsync(ReturnLogs entity) => _creatable.AddAsync(entity);

        public Task<List<ReturnLogs>> GetAllReturnLogsAsync(ReturnLogsPaginationDto pagination)
        {
            string receiptId = pagination.ReceiptId?.Trim() ?? string.Empty;
            string employeeId = pagination.EmployeeId?.Trim() ?? string.Empty;
            string customerId = pagination.CustomerId?.Trim() ?? string.Empty;

            var query = _context.ReturnLogs
                .AsNoTracking()
                .Include(r => r.Employee)
                .Include(r => r.Customer)
                .Include(r => r.Lines).ThenInclude(l => l.Product)
                .AsSplitQuery()
                .Where(r => r.ReceiptId.Contains(receiptId) &&
                            r.Employee!.EmployeeId.Contains(employeeId) &&
                            (customerId.Length == 0 || (r.Customer != null && r.Customer.CustomerId.Contains(customerId))));

            bool descending = pagination.OrderType == OrderType.Descending;
            query = pagination.OrderBy switch
            {
                ReturnLogsOrderBy.RefundAmount => descending ? query.OrderByDescending(r => (double)r.RefundAmount) : query.OrderBy(r => (double)r.RefundAmount),
                ReturnLogsOrderBy.ReceiptId => descending ? query.OrderByDescending(r => r.ReceiptId) : query.OrderBy(r => r.ReceiptId),
                _ => descending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
            };

            return query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }
    }
}
