using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock
{
    public interface ISalesRepository : 
        IEfRepository<Sales>, 
        IEfUpdatableRepository<Sales>, 
        IEfCreatableRepository<Sales>, 
        IEfSoftDeletableRepository<Sales>
    {
        Task<IEnumerable<Sales>> GetAllSalesAsync(SalesPaginationDto pagination);
    }
}
