using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Services.ManagementApp.Stock
{
    public class ProductService(IProductRepository productRepository) : IProductService
    {
        public async Task CreateProductAsync(CreateProductDto product)
        {
            if (product == null) throw new ArgumentNullException("Product cant be null.");

            await productRepository.AddAsync(product.ToProductEntity());
        }

        public async Task<List<ProductDto>> GetAllProductsAsync(ProductPaginationDto pagination)
        {
            var products = await productRepository.GetAllProductsAsync(pagination);

            var result = new List<ProductDto>();
            foreach (var product in products)
            {
                result.Add(product.ToProductDto());
            }
            return result;
        }

        public async Task SoftDeleteProductAsync(Guid id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException($"Product with the id {id} not found.");

            await productRepository.SoftDeleteAsync(product);
        }

        public async Task UpdateProductAsync(UpdateProductDto productDto)
        {
            var product = await productRepository.GetByIdAsync(productDto.Id);
            if (product == null) throw new KeyNotFoundException($"Product with the id {productDto.Id} not found.");

            product.ToProductEntity(productDto);
            await productRepository.UpdateAsync(product);
        }
    }
}
