using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock
{
    public interface IProductRepository : 
        IEfRepository<Product>, 
        IEfUpdatableRepository<Product>, 
        IEfCreatableRepository<Product>, 
        IEfSoftDeletableRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(ProductPaginationDto pagination);
    }
}
