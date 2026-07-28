using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock
{
    public interface IShelfRepository : 
        IEfRepository<Shelf>, 
        IEfUpdatableRepository<Shelf>, 
        IEfCreatableRepository<Shelf>, 
        IEfSoftDeletableRepository<Shelf>
    {
        Task<IEnumerable<Shelf>> GetAllShelvesAsync(ShelfPaginationDto pagination);
    }
}
