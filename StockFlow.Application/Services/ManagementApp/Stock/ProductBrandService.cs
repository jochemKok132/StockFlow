using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.ProductBrand;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Services.ManagementApp.Stock
{
    public class ProductBrandService(IProductBrandRepository productBrandRepository) : IProductBrandService
    {
        public async Task CreateProductBrandAsync(CreateProductBrandDto productBrand)
        {
            if (productBrand == null) throw new ArgumentNullException("ProductBrand cant be null.");

            await productBrandRepository.AddAsync(productBrand.ToProductBrandEntity());
        }

        public async Task<List<ProductBrandDto>> GetAllProductBrandsAsync(ProductBrandPaginationDto pagination)
        {
            var productBrands = await productBrandRepository.GetAllProductBrandsAsync(pagination);

            var result = new List<ProductBrandDto>();
            foreach (var productBrand in productBrands)
            {
                result.Add(productBrand.ToProductBrandDto());
            }
            return result;
        }

        public async Task SoftDeleteProductBrandAsync(Guid id)
        {
            var productBrand = await productBrandRepository.GetByIdAsync(id);
            if (productBrand == null) throw new KeyNotFoundException($"ProductBrand with the id {id} not found.");

            await productBrandRepository.SoftDeleteAsync(productBrand);
        }

        public async Task UpdateProductBrandAsync(UpdateProductBrandDto productBrandDto)
        {
            var productBrand = await productBrandRepository.GetByIdAsync(productBrandDto.Id);
            if (productBrand == null) throw new KeyNotFoundException($"ProductBrand with the id {productBrandDto.Id} not found.");

            productBrand.ToProductBrandEntity(productBrandDto);
            await productBrandRepository.UpdateAsync(productBrand);
        }
    }
}
