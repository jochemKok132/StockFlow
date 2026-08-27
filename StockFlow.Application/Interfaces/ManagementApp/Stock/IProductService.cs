using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.ManagementApp.Stock
{
    public interface IProductService
    {
        Task<List<ProductBulkViewDto>> GetAllProductsAsync(ProductPaginationDto pagination);
        Task<ProductDto> GetProductDetailsAsync(Guid id);
        Task CreateProductAsync(CreateProductDto product);
        Task UpdateProductAsync(UpdateProductDto product);
        Task SoftDeleteProductAsync(Guid id);
    }
}
