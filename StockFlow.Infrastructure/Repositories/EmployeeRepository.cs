using Microsoft.EntityFrameworkCore;
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
    }
}
