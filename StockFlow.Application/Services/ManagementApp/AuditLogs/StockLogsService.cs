using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Application.Validations;
using StockFlow.Application.Validations.AuditLogs;
using StockFlow.Shared.Common;

namespace StockFlow.Application.Services.ManagementApp.AuditLogs
{
    public class StockLogsService(IStockLogsRepository stockLogsRepository, IProductRepository productRepository) : IStockLogsService
    {
        public async Task CreateStockLogsAsync(CreateStockLogsDto log)
        {
            log.Validate();

            await stockLogsRepository.AddAsync(log.ToStockLogsEntity());
        }

        public async Task<ProductStockHistoryDto> GetProductStockHistoryAsync(int barcode, ProductHistoryPaginationDto pagination)
        {
            pagination.Validate();
            if (!IdentifierFormats.IsProductBarcode(barcode.ToString()))
                throw new ArgumentException($"A product barcode must be {IdentifierFormats.ProductBarcodeLength} digits.");

            var product = await productRepository.GetProductByBarcodeAsync(barcode);
            if (product == null) throw new KeyNotFoundException($"Product with the barcode {barcode} not found.");

            var productDto = product.ToProductDto();
            productDto.Brand = product.Brand?.ToProductBrandDto();
            productDto.Shelf = product.Shelf?.ToShelfDto();

            var logs = (await stockLogsRepository.GetLogsByIdentifierAsync(StockLogType.Product, barcode.ToString()))
                .Select(l => l.ToStockLogsDto())
                .Select(l => (Log: l, Change: StockLogMessages.StockChange(l.Messages)))
                .ToList();
            var matching = pagination.StockOnly ? logs.Where(l => l.Change.HasValue).ToList() : logs;

            int Units(Func<LogTypeDto, bool> type, int sign) =>
                logs.Where(l => type(l.Log.LogType) && l.Change * sign > 0).Sum(l => l.Change!.Value * sign);

            return new ProductStockHistoryDto
            {
                Product = productDto,
                Logs = matching
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .Select(l => l.Log)
                    .ToList(),
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                MatchingLogs = matching.Count,
                TotalLogs = logs.Count,
                UnitsSold = Units(t => t == LogTypeDto.Sold, -1),
                UnitsReturned = Units(t => t == LogTypeDto.Returned, 1),
                UnitsWrittenOff = Units(t => t == LogTypeDto.WrittenOff, -1),
                UnitsAdded = Units(t => t != LogTypeDto.Sold && t != LogTypeDto.Returned, 1),
            };
        }

        public async Task<List<StockLogsDto>> GetAllStockLogsAsync(StockLogsPaginationDto pagination)
        {
            pagination.Validate();

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
