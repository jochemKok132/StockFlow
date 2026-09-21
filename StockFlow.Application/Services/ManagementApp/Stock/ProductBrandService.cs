using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.ProductBrand;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Services.ManagementApp.Stock
{
    public class ProductBrandService(IProductBrandRepository productBrandRepository, IStockLogsService stockLogsService) : IProductBrandService
    {
        public async Task CreateProductBrandAsync(CreateProductBrandDto productBrand, Guid employeeId)
        {
            if (productBrand == null) throw new ArgumentNullException("ProductBrand cant be null.");
            await productBrandRepository.AddAsync(productBrand.ToProductBrandEntity());
            await stockLogsService.CreateStockLogsAsync(productBrand.ToCreateStockLogsDto(employeeId));
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

        public async Task SoftDeleteProductBrandAsync(Guid id, Guid employeeId)
        {
            var productBrand = await productBrandRepository.GetByIdAsync(id);
            if (productBrand == null) throw new KeyNotFoundException($"ProductBrand with the id {id} not found.");
            await stockLogsService.CreateStockLogsAsync(productBrand.ToSoftDeleteStockLogsDto(employeeId));

            await productBrandRepository.SoftDeleteAsync(productBrand);
        }

        public async Task UpdateProductBrandAsync(UpdateProductBrandDto productBrandDto, Guid employeeId)
        {
            var productBrand = await productBrandRepository.GetByIdAsync(productBrandDto.Id);
            if (productBrand == null) throw new KeyNotFoundException($"ProductBrand with the id {productBrandDto.Id} not found.");

            productBrand.ToProductBrandEntity(productBrandDto);
            await stockLogsService.CreateStockLogsAsync(productBrandDto.ToUpdateStockLogsDto(productBrand, employeeId));
            await productBrandRepository.UpdateAsync(productBrand);
        }
    }
}
