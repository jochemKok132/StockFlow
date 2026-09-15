using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.Repositories.ManagementApp
{
    public interface ICustomerRepository :
        IEfRepository<Customer>,
        IEfUpdatableRepository<Customer>,
        IEfCreatableRepository<Customer>,
        IEfSoftDeletableRepository<Customer>
    {
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<IEnumerable<Customer>?> GetAllCustomersAsync(CustomerPaginationDto pagination);

    }
}
