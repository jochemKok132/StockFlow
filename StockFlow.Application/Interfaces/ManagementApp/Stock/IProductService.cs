using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.ManagementApp.Stock
{
    public interface IProductService
    {
        Task<List<ProductBulkViewDto>> GetAllProductsAsync(ProductPaginationDto pagination);
        Task<ProductDto> GetProductDetailsAsync(Guid id);
        Task CreateProductAsync(CreateProductDto product, Guid employeeId);
        Task UpdateProductAsync(UpdateProductDto product, Guid employeeId);
        Task SoftDeleteProductAsync(Guid id, Guid employeeId);
        Task UpdateProductStockAsync(Guid id, int amount, Guid employeeId, LogTypeDto logType = LogTypeDto.Sold);
        Task<ProductDto> GetProductByBarcodeAsync(int barcode);
        Task<int> WriteOffStockAsync(WriteOffRequest request, Guid employeeId);
        Task<ProductDto> GetSellableProductByBarcodeAsync(int barcode);
        Task EnsureSellableAsync(IEnumerable<Guid> productIds);
    }
}
