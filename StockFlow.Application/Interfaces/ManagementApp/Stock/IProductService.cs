using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.ManagementApp.Stock
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProductsAsync(ProductPaginationDto pagination);
        Task CreateProductAsync(CreateProductDto product);
        Task UpdateProductAsync(UpdateProductDto product);
        Task SoftDeleteProductAsync(Guid id);
    }
}
