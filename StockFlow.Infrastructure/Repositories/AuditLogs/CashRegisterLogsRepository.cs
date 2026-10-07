using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories.EfRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockFlow.Infrastructure.Repositories.Stock
{
    public class CashRegisterLogsRepository :
        EfRepository<CashRegisterLogs>,
        IEfCreatableRepository<CashRegisterLogs>,
        ICashRegisterLogsRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<CashRegisterLogs> _creatable;

        public CashRegisterLogsRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<CashRegisterLogs>(context);
        }

        public async Task AddAsync(CashRegisterLogs log)
        {
            log.CreatedAt = DateTime.UtcNow;
            _context.CashRegisterLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CashRegisterLogs>> GetAllCashRegisterLogsAsync(CashRegisterLogsPaginationDto pagination)
        {
            var employeeId = pagination.EmployeeId?.ToLower() ?? string.Empty;
            var employeeName = pagination.EmployeeName?.ToLower() ?? string.Empty;
            var email = pagination.Email?.ToLower() ?? string.Empty;
            var customerName = pagination.CustomerName?.ToLower() ?? string.Empty;

            var noCustomerFilter = email.Length == 0 && customerName.Length == 0;

            var query = _context.CashRegisterLogs
                .AsNoTracking()
                .Include(e => e.ProductsSold).ThenInclude(e => e.Product)
                .Include(e => e.ProductsSold).ThenInclude(e => e.Sale)
                .Include(e => e.Customer)
                .Include(e => e.Employee)
                .Where(e =>
                    e.Employee.EmployeeId.ToLower().Contains(employeeId) &&
                    e.Employee.FullName.ToLower().Contains(employeeName) &&
                    (e.Customer == null
                        ? noCustomerFilter
                        : e.Customer.Email.ToLower().Contains(email) &&
                          e.Customer.FullName.ToLower().Contains(customerName)));

            query = pagination.OrderType == OrderType.Descending
                ? pagination.OrderBy switch
                {
                    CashRegisterLogsOrderBy.CustomerName => query.OrderByDescending(p => p.Customer.FullName),
                    CashRegisterLogsOrderBy.TotalOff => query.OrderByDescending(p => p.TotalOff),
                    CashRegisterLogsOrderBy.TotalProducts => query.OrderByDescending(p => p.ProductsSold),
                    CashRegisterLogsOrderBy.EmployeeId => query.OrderByDescending(p => p.EmployeeId),
                    CashRegisterLogsOrderBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt),
                }
                : pagination.OrderBy switch
                {
                    CashRegisterLogsOrderBy.CustomerName => query.OrderBy(p => p.Customer.FullName),
                    CashRegisterLogsOrderBy.TotalOff => query.OrderBy(p => p.TotalOff),
                    CashRegisterLogsOrderBy.TotalProducts => query.OrderBy(p => p.ProductsSold),
                    CashRegisterLogsOrderBy.EmployeeId => query.OrderBy(p => p.EmployeeId),
                    CashRegisterLogsOrderBy.CreatedAt => query.OrderBy(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.CreatedAt),
                };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }
    }
}