using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.Repositories.ManagementApp
{
    public interface IEmployeeRepository :
        IEfRepository<Employee>,
        IEfUpdatableRepository<Employee>,
        IEfCreatableRepository<Employee>
    {
        Task<Employee?> GetEmployeeByEmployeeIdAsync(string employeeId);
        Task<string> GetNewEmployeeIdAsync();
    }
}
