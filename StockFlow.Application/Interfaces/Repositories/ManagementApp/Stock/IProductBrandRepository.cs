using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock
{
    public interface IProductBrandRepository : 
        IEfRepository<ProductBrand>, 
        IEfUpdatableRepository<ProductBrand>, 
        IEfCreatableRepository<ProductBrand>, 
        IEfSoftDeletableRepository<ProductBrand>
    {
        Task<IEnumerable<ProductBrand>> GetAllProductBrandsAsync(ProductBrandPaginationDto pagination);
    }
}
