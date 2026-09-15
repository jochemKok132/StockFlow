using Microsoft.EntityFrameworkCore;
using StockFlow.Application.DTOs.Enums;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories;
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
    public class EmployeeRepository :
        EfRepository<Employee>,
        IEfUpdatableRepository<Employee>,
        IEfCreatableRepository<Employee>,
        IEmployeeRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<Employee> _creatable;
        private readonly EfUpdatableRepository<Employee> _updatable;

        public EmployeeRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<Employee>(context);
            _updatable = new EfUpdatableRepository<Employee>(context);
        }

        public Task AddAsync(Employee entity) => _creatable.AddAsync(entity);
        public Task UpdateAsync(Employee entity) => _updatable.UpdateAsync(entity);

        public Task<Employee?> GetEmployeeByEmployeeIdAsync(string employeeId)
        {
            return _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<string> GetNewEmployeeIdAsync()
        {
            var count = await _context.Employees.CountAsync();
            return (count + 1).ToString("D6");
        }
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(EmployeePaginationDto pagination)
        {
            var query = _context.Employees
                .AsNoTracking()
                .Where(e =>
                e.FullName.ToLower().Contains((pagination.EmployeeName ?? string.Empty).ToLower()) &&
                e.EmployeeId.ToLower().Contains((pagination.EmployeeId ?? string.Empty).ToLower()) &&
                e.Role.ToString().ToLower().Contains((pagination.Role.ToString() ?? string.Empty).ToLower()));

            query = pagination.OrderType == OrderType.Descending
                ? pagination.OrderBy switch
                {
                    EmployeeOrderBy.EmployeeName => query.OrderByDescending(p => p.FullName),
                    EmployeeOrderBy.EmployeeId => query.OrderByDescending(p => p.EmployeeId),
                    EmployeeOrderBy.Role => query.OrderByDescending(p => p.Role),
                    EmployeeOrderBy.CreatedAt => query.OrderByDescending(p => p.CreatedAt),
                    EmployeeOrderBy.UpdatedAt => query.OrderByDescending(p => p.UpdatedAt),
                    _ => query.OrderByDescending(p => p.CreatedAt),
                }
                : pagination.OrderBy switch
                {
                    EmployeeOrderBy.EmployeeName => query.OrderBy(p => p.FullName),
                    EmployeeOrderBy.EmployeeId => query.OrderBy(p => p.EmployeeId),
                    EmployeeOrderBy.Role => query.OrderBy(p => p.Role),
                    EmployeeOrderBy.CreatedAt => query.OrderBy(p => p.CreatedAt),
                    EmployeeOrderBy.UpdatedAt => query.OrderBy(p => p.UpdatedAt),
                    _ => query.OrderBy(p => p.CreatedAt),
                };

            return await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
        }
    }
}
