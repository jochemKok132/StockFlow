using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
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
using StockFlow.Application.Validations;
using StockFlow.Application.Validations.Stock;

namespace StockFlow.Application.Services.ManagementApp.Stock
{
    public class ProductService(IProductRepository productRepository, ISalesRepository salesRepository, IShelfRepository shelfRepository, IProductBrandRepository productBrandRepository, IStockLogsService stockLogsService) : IProductService
    {

        public async Task CreateProductAsync(CreateProductDto product, Guid employeeId)
        {
            product.Validate();
            await EnsureBrandAndShelfExistAsync(product.BrandId, product.ShelfId);

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

        private async Task EnsureBrandAndShelfExistAsync(Guid brandId, Guid shelfId)
        {
            var brand = await productBrandRepository.GetByIdAsync(brandId);
            if (brand is null || brand.SoftDeleted)
                throw new ArgumentException("The chosen brand doesn't exist.");

            var shelf = await shelfRepository.GetByIdAsync(shelfId);
            if (shelf is null || shelf.SoftDeleted)
                throw new ArgumentException("The chosen shelf doesn't exist.");
        }

        private async Task<List<string>> DescribeBrandAndShelfChangesAsync(Product product, UpdateProductDto productDto)
        {
            List<string> messages = [];

            if (product.BrandId != productDto.BrandId)
            {
                var oldBrand = await productBrandRepository.GetByIdAsync(product.BrandId);
                var newBrand = await productBrandRepository.GetByIdAsync(productDto.BrandId);
                messages.Add($"BrandName: New: {newBrand?.BrandName ?? "-"}. Old: {oldBrand?.BrandName ?? "-"}");
            }

            if (product.ShelfId != productDto.ShelfId)
            {
                var oldShelf = await shelfRepository.GetByIdAsync(product.ShelfId);
                var newShelf = await shelfRepository.GetByIdAsync(productDto.ShelfId);
                messages.Add($"ShelfName: New: {newShelf?.ShelfName ?? "-"}. Old: {oldShelf?.ShelfName ?? "-"}");
            }

            return messages;
        }

        public async Task<List<ProductBulkViewDto>> GetAllProductsAsync(ProductPaginationDto pagination)
        {
            pagination.Validate();

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
            if (product == null) throw new KeyNotFoundException("Product not found.");
            await stockLogsService.CreateStockLogsAsync(product.ToSoftDeleteStockLogsDto(employeeId));
            await productRepository.SoftDeleteAsync(product);
        }

        public async Task UpdateProductAsync(UpdateProductDto productDto, Guid employeeId)
        {
            productDto.Validate();
            await EnsureBrandAndShelfExistAsync(productDto.BrandId, productDto.ShelfId);

            var product = await productRepository.GetByIdAsync(productDto.Id);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            var log = productDto.ToUpdateStockLogsDto(product, employeeId);
            log.Messages.AddRange(await DescribeBrandAndShelfChangesAsync(product, productDto));

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
            if (product == null) throw new KeyNotFoundException("Product not found.");
            var productDto = product.ToProductDto();
            productDto.Brand = (await productBrandRepository.GetByIdAsync(productDto.BrandId))?.ToProductBrandDto();
            productDto.Shelf = (await shelfRepository.GetByIdAsync(productDto.ShelfId))?.ToShelfDto();
            return productDto;

        }

        public async Task<ProductDto> GetProductByBarcodeAsync(int barcode)
        {
            var product = await productRepository.GetProductByBarcodeAsync(barcode);
            if (product == null) throw new KeyNotFoundException($"Product with the barcode {barcode} not found.");
            var productDto = product.ToProductDto();
            productDto.Brand = (await productBrandRepository.GetByIdAsync(productDto.BrandId))?.ToProductBrandDto();
            productDto.Shelf = (await shelfRepository.GetByIdAsync(productDto.ShelfId))?.ToShelfDto();
            return productDto;
        }

        public async Task<int> WriteOffStockAsync(WriteOffRequest request, Guid employeeId)
        {
            request.Validate();

            var product = await productRepository.GetByIdAsync(request.ProductId);
            if (product == null || product.SoftDeleted) throw new KeyNotFoundException("Product not found.");

            var log = product.ChangeStock(-request.Amount, employeeId, LogTypeDto.WrittenOff);
            log.Messages.Add($"Reason: {request.Reason}");
            if (!string.IsNullOrWhiteSpace(request.Note))
                log.Messages.Add($"Note: {request.Note.Trim()}");

            product.ChangeStock(-request.Amount);

            await productRepository.UpdateAsync(product);
            await stockLogsService.CreateStockLogsAsync(log);

            return product.Stock;
        }

        public async Task UpdateProductStockAsync(Guid id, int amount, Guid employeeId, LogTypeDto logType = LogTypeDto.Sold)
        {
            ProductValidation.ValidateStockChange(amount);

            var product = await productRepository.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            var log = product.ChangeStock(amount, employeeId, logType);

            product.ChangeStock(amount);

            await productRepository.UpdateAsync(product);
            await stockLogsService.CreateStockLogsAsync(log);
        }
    }
}
