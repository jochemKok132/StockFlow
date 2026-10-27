using StockFlow.Shared.Common;
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
    public class ProductService(IProductRepository productRepository, ISalesRepository salesRepository, IShelfRepository shelfRepository, IProductBrandRepository productBrandRepository, IStockLogsService stockLogsService, ISupplierRepository supplierRepository) : IProductService
    {
        public async Task CreateProductAsync(CreateProductDto product, Guid employeeId)
        {
            product.Validate();
            await EnsureReferencesExistAsync(product.BrandId, product.ShelfId, product.SupplierId);

            var productEntity = product.ToProductEntity();

            var matchingSales = await salesRepository.GetAllSalesForProductTags(product.SalesTags.ToList());
            foreach (var sale in matchingSales)
            {
                productEntity.Sales.Add(sale);
            }

            productEntity.Prices.Add(NewPrice(productEntity.Id, productEntity.Price, employeeId));
            productEntity.Barcode = await productRepository.GetNewBarcode();
            await productRepository.AddAsync(productEntity);
            await stockLogsService.CreateStockLogsAsync(product.ToCreateStockLogsDto(employeeId, productEntity.Barcode));
        }

        private static ProductPrice NewPrice(Guid productId, decimal price, Guid employeeId) => new()
        {
                    ProductId = productId,
            Price = price,
            ValidFrom = DateTime.Today,
            CreatedAt = DateTime.UtcNow,
            CreatedById = employeeId,
        };

        private async Task EnsureReferencesExistAsync(Guid brandId, Guid shelfId, Guid supplierId)
        {
            var brand = await productBrandRepository.GetByIdAsync(brandId);
            if (brand is null || brand.SoftDeleted)
                throw new ArgumentException("The chosen brand doesn't exist.");

            var shelf = await shelfRepository.GetByIdAsync(shelfId);
            if (shelf is null || shelf.SoftDeleted)
                throw new ArgumentException("The chosen shelf doesn't exist.");

            var supplier = await supplierRepository.GetByIdAsync(supplierId);
            if (supplier is null || supplier.SoftDeleted)
                throw new ArgumentException("The chosen supplier doesn't exist.");
        }

        private async Task<List<string>> DescribeReferenceChangesAsync(Product product, UpdateProductDto productDto)
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

            if (product.SupplierId != productDto.SupplierId)
            {
                var oldSupplier = await supplierRepository.GetByIdAsync(product.SupplierId);
                var newSupplier = await supplierRepository.GetByIdAsync(productDto.SupplierId);
                messages.Add($"SupplierName: New: {newSupplier?.SupplierName ?? "-"}. Old: {oldSupplier?.SupplierName ?? "-"}");
            }

            return messages;
        }

        public async Task<List<ProductBulkViewDto>> GetAllProductsAsync(ProductPaginationDto pagination)
        {
            pagination.Validate();
            var products = await productRepository.GetAllProductsAsync(pagination);
            return products.Select(p => p.ToProductBulkViewDto()).ToList();
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
            await EnsureReferencesExistAsync(productDto.BrandId, productDto.ShelfId, productDto.SupplierId);

            var product = await productRepository.GetByIdAsync(productDto.Id);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            var log = productDto.ToUpdateStockLogsDto(product, employeeId);
            log.Messages.AddRange(await DescribeReferenceChangesAsync(product, productDto));

            bool priceChanged = product.Price != productDto.Price;
            product.ToProductEntity(productDto);
            if (priceChanged)
                product.Prices.Add(NewPrice(product.Id, productDto.Price, employeeId));
            if (!product.CanExpire && product.Batches.Count > 0)
                product.Batches.Clear();

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

            return await ToDetailsAsync(product);
        }

        private async Task<ProductDto> ToDetailsAsync(Product product)
        {
            var productDto = product.ToProductDto();
            productDto.Brand = (await productBrandRepository.GetByIdAsync(productDto.BrandId))?.ToProductBrandDto();
            productDto.Shelf = (await shelfRepository.GetByIdAsync(productDto.ShelfId))?.ToShelfDto();
            productDto.SupplierName ??= (await supplierRepository.GetByIdAsync(productDto.SupplierId))?.SupplierName;
            productDto.ReservedStock = (await productRepository.GetReservedStockAsync([product.Id])).GetValueOrDefault(product.Id);
            productDto.OnOrder = (await productRepository.GetOnOrderAsync([product.Id])).GetValueOrDefault(product.Id);
            return productDto;
        }

        public async Task<ProductDto> GetProductByBarcodeAsync(int barcode)
        {
            var product = await productRepository.GetProductByBarcodeAsync(barcode);
            if (product == null) throw new KeyNotFoundException($"Product with the barcode {barcode} not found.");

            decimal current = product.CurrentPrice(DateTime.Today);
            if (current != product.Price)
            {
                product.Price = current;
                await productRepository.UpdateAsync(product);
            }

            return await ToDetailsAsync(product);
        }

        public async Task<ProductDto> GetSellableProductByBarcodeAsync(int barcode)
        {
            var product = await GetProductByBarcodeAsync(barcode);
            if (product.SoftDeleted)
                throw new KeyNotFoundException($"Product with the barcode {barcode} not found.");

            string? refusal = ProductRules.SaleRefusal(product.ProductName, product.Status == ProductStatusDto.Active, product.Status == ProductStatusDto.Draft, product.Recalled);
            if (refusal is not null)
                throw new ArgumentException(refusal);

            return product;
        }

        public async Task EnsureSellableAsync(IEnumerable<Guid> productIds)
        {
            var ids = productIds.Distinct().ToList();
            var products = await productRepository.GetByIdsAsync(ids);
            foreach (var id in ids)
            {
                var product = products.FirstOrDefault(p => p.Id == id);
                if (product is null || product.SoftDeleted)
                    throw new ArgumentException("A product on this receipt doesn't exist anymore.");

                string? refusal = product.SaleRefusal();
                if (refusal is not null)
                    throw new ArgumentException(refusal);
            }
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
            if (product.CanExpire)
                product.ConsumeBatches(request.Amount);

            await productRepository.UpdateAsync(product);
            await stockLogsService.CreateStockLogsAsync(log);
            return product.Stock;
        }

        public Task UpdateProductStockAsync(Guid id, int amount, Guid employeeId, LogTypeDto logType = LogTypeDto.Sold) =>
            ChangeStockAsync(id, amount, employeeId, logType, []);

        public Task SellAsync(Guid id, int amount, Guid employeeId, string receiptId) =>
            ChangeStockAsync(id, -amount, employeeId, LogTypeDto.Sold, [$"Receipt: {receiptId}"]);

        private async Task ChangeStockAsync(Guid id, int amount, Guid employeeId, LogTypeDto logType, List<string> extraMessages)
        {
            ProductValidation.ValidateStockChange(amount);

            var product = await productRepository.GetByIdAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found.");

            var log = product.ChangeStock(amount, employeeId, logType);
            log.Messages.AddRange(extraMessages);
            product.ChangeStock(amount);
            if (amount < 0 && product.CanExpire)
                product.ConsumeBatches(-amount);

            await productRepository.UpdateAsync(product);
            await stockLogsService.CreateStockLogsAsync(log);
        }
    }
}
