using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.ProductBrand;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.ManagementApp.Stock
{
    public interface IProductBrandService
    {
        Task<List<ProductBrandDto>> GetAllProductBrandsAsync(ProductBrandPaginationDto pagination);
        Task CreateProductBrandAsync(CreateProductBrandDto product, Guid employeeId);
        Task UpdateProductBrandAsync(UpdateProductBrandDto product, Guid employeeId);
        Task SoftDeleteProductBrandAsync(Guid id, Guid employeeId);
    }
}
