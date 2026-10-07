using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Application.Services.ManagementApp.AuditLogs;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Services.ManagementApp.Stock
{
    public class ProductService(IProductRepository productRepository, ISalesRepository salesRepository, IShelfRepository shelfRepository, IProductBrandRepository productBrandRepository, IStockLogsService stockLogsService) : IProductService
    {

        public async Task CreateProductAsync(CreateProductDto product, Guid employeeId)
        {
            if (product == null) throw new ArgumentNullException("Product cant be null.");

            var productEntity = product.ToProductEntity();

            var matchingSales = await salesRepository.GetAllSalesForProductTags(product.SalesTags.ToList());

            foreach (var sale in matchingSales)
            {
                productEntity.Sales.Add(sale);
            }
            productEntity.Barcode = await productRepository.GetNewBarcode();


            await productRepository.AddAsync(productEntity);
            await stockLogsService.CreateStockLogsAsync(product.ToCreateStockLogsDto(employeeId, productEntity.Barcode));
        }

        public async Task<List<ProductBulkViewDto>> GetAllProductsAsync(ProductPaginationDto pagination)
        {
            var products = await productRepository.GetAllProductsAsync(pagination);

            var result = new List<ProductBulkViewDto>();
            foreach (var product in products)
            {
                result.Add(product.ToProductBulkViewDto());
            }
            return result;
        }

        public async Task SoftDeleteProductAsync(Guid id, Guid employeeId)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException($"Product with the id {id} not found.");
            await stockLogsService.CreateStockLogsAsync(product.ToSoftDeleteStockLogsDto(employeeId));
            await productRepository.SoftDeleteAsync(product);
        }

        public async Task UpdateProductAsync(UpdateProductDto productDto, Guid employeeId)
        {
            var product = await productRepository.GetByIdAsync(productDto.Id);
            if (product == null) throw new KeyNotFoundException($"Product with the id {productDto.Id} not found.");

            var log = productDto.ToUpdateStockLogsDto(product, employeeId);

            product.ToProductEntity(productDto);

            var matchingSales = (await salesRepository.GetAllSalesForProductTags(product.SalesTags.ToList())).ToList();
            var matchingIds = matchingSales.Select(s => s.Id).ToHashSet();

            foreach (var old in product.Sales.Where(s => !matchingIds.Contains(s.Id)).ToList())
                product.Sales.Remove(old);

            foreach (var sale in matchingSales)
                if (!product.Sales.Any(s => s.Id == sale.Id))
                    product.Sales.Add(sale);

            await productRepository.UpdateAsync(product);
            await stockLogsService.CreateStockLogsAsync(log);
        }
        public async Task<ProductDto> GetProductDetailsAsync(Guid id) 
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException($"Product with the id {id} not found.");
            var productDto = product.ToProductDto();
            productDto.Brand = (await productBrandRepository.GetByIdAsync(productDto.BrandId)).ToProductBrandDto();
            productDto.Shelf = (await shelfRepository.GetByIdAsync(productDto.ShelfId)).ToShelfDto();
            return productDto;

        }

        public async Task<ProductDto> GetProductByBarcodeAsync(int barcode)
        {
            var product = await productRepository.GetProductByBarcodeAsync(barcode);
            if (product == null) throw new KeyNotFoundException($"Product with the barcode {barcode} not found.");
            var productDto = product.ToProductDto();
            productDto.Brand = (await productBrandRepository.GetByIdAsync(productDto.BrandId)).ToProductBrandDto();
            productDto.Shelf = (await shelfRepository.GetByIdAsync(productDto.ShelfId)).ToShelfDto();
            return productDto;
        }

        public async Task UpdateProductStockAsync(Guid id, int amount, Guid employeeId)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException($"Product with the id {id} not found.");

            var log = product.ChangeStock(amount, employeeId);

            product.ChangeStock(amount);

            await productRepository.UpdateAsync(product);
            await stockLogsService.CreateStockLogsAsync(log);
        }
    }
}
