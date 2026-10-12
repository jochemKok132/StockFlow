using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Services.ManagementApp.AuditLogs
{
    // Uses the product repository (not IProductService) because ProductService already depends on this service.
    public class StockLogsService(IStockLogsRepository stockLogsRepository, IProductRepository productRepository) : IStockLogsService
    {
        public async Task CreateStockLogsAsync(CreateStockLogsDto log)
        {
            if (log == null) throw new ArgumentNullException("StockLog cant be null.");

            await stockLogsRepository.AddAsync(log.ToStockLogsEntity());
        }

        public async Task<ProductStockHistoryDto> GetProductStockHistoryAsync(int barcode)
        {
            var product = await productRepository.GetProductByBarcodeAsync(barcode);
            if (product == null) throw new KeyNotFoundException($"Product with the barcode {barcode} not found.");

            var productDto = product.ToProductDto();
            productDto.Brand = product.Brand?.ToProductBrandDto();
            productDto.Shelf = product.Shelf?.ToShelfDto();

            var logs = await stockLogsRepository.GetLogsByIdentifierAsync(StockLogType.Product, barcode.ToString());

            return new ProductStockHistoryDto
            {
                Product = productDto,
                Logs = logs.Select(l => l.ToStockLogsDto()).ToList(),
            };
        }

        public async Task<List<StockLogsDto>> GetAllStockLogsAsync(StockLogsPaginationDto pagination)
        {
            var stockLogs = await stockLogsRepository.GetAllStockLogsAsync(pagination);

            var result = new List<StockLogsDto>();
            foreach (var stockLog in stockLogs)
            {
                result.Add(stockLog.ToStockLogsDto());
            }
            return result;
        }

    }
}
