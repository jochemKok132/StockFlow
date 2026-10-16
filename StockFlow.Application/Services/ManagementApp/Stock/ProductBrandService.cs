using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Stock.ProductBrand;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Application.Validations;
using StockFlow.Application.Validations.Stock;

namespace StockFlow.Application.Services.ManagementApp.Stock
{
    public class ProductBrandService(IProductBrandRepository productBrandRepository, IStockLogsService stockLogsService) : IProductBrandService
    {
        public async Task CreateProductBrandAsync(CreateProductBrandDto productBrand, Guid employeeId)
        {
            productBrand.Validate();
            await productBrandRepository.AddAsync(productBrand.ToProductBrandEntity());
            await stockLogsService.CreateStockLogsAsync(productBrand.ToCreateStockLogsDto(employeeId));
        }

        public async Task<List<ProductBrandDto>> GetAllProductBrandsAsync(ProductBrandPaginationDto pagination)
        {
            pagination.Validate();

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
            if (productBrand == null) throw new KeyNotFoundException("Brand not found.");
            await stockLogsService.CreateStockLogsAsync(productBrand.ToSoftDeleteStockLogsDto(employeeId));

            await productBrandRepository.SoftDeleteAsync(productBrand);
        }

        public async Task UpdateProductBrandAsync(UpdateProductBrandDto productBrandDto, Guid employeeId)
        {
            productBrandDto.Validate();

            var productBrand = await productBrandRepository.GetByIdAsync(productBrandDto.Id);
            if (productBrand == null) throw new KeyNotFoundException("Brand not found.");

            var log = productBrandDto.ToUpdateStockLogsDto(productBrand, employeeId);

            productBrand.ToProductBrandEntity(productBrandDto);
            await stockLogsService.CreateStockLogsAsync(log);
            await productBrandRepository.UpdateAsync(productBrand);
        }
    }
}
