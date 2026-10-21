using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Stock.Sales;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Application.Validations;
using StockFlow.Application.Validations.Stock;

namespace StockFlow.Application.Services.ManagementApp.Stock
{
    public class SalesService(ISalesRepository salesRepository, IProductRepository productRepository, IStockLogsService stockLogsService) : ISalesService
    {

        public async Task CreateSalesAsync(CreateSalesDto sale, Guid employeeId)
        {
            sale.Validate();

            var salesEntity = sale.ToSalesEntity();

            var matchingProducts = await productRepository.GetAllProductsWithTagsAsync(sale.SalesTags.ToList());

            foreach (var product in matchingProducts)
            {
                salesEntity.Products.Add(product);
            }
            await salesRepository.AddAsync(salesEntity);
            await stockLogsService.CreateStockLogsAsync(sale.ToCreateStockLogsDto(employeeId));
        }

        public async Task<List<SalesDto>> GetAllSalesAsync(SalesPaginationDto pagination)
        {
            pagination.Validate();

            var sales = (await salesRepository.GetAllSalesAsync(pagination, DateTime.Today)).ToList();
            var itemsSold = await salesRepository.GetItemsSoldAsync(sales.Select(s => s.Id));

            var result = new List<SalesDto>();
            foreach (var sale in sales)
            {
                var dto = sale.ToSalesDto();
                dto.ItemsSold = itemsSold.GetValueOrDefault(sale.Id);
                result.Add(dto);
            }
            return result;
        }

        public async Task SoftDeleteSalesAsync(Guid id, Guid employeeId)
        {
            var sale = await salesRepository.GetByIdAsync(id);
            if (sale == null) throw new KeyNotFoundException("Sale not found.");
            if (sale.SaleType == SaleType.Moving) throw new ArgumentException("The moving sale can't be deleted. Change its percentage instead.");
            await salesRepository.SoftDeleteAsync(sale);
            await stockLogsService.CreateStockLogsAsync(sale.ToSoftDeleteStockLogsDto(employeeId));
        }

        public async Task UpdateSalesAsync(UpdateSalesDto salesDto, Guid employeeId)
        {
            Guard.NotNull(salesDto, "Sale");

            var sale = await salesRepository.GetByIdWithProductsAsync(salesDto.Id);
            if (sale == null) throw new KeyNotFoundException("Sale not found.");

            bool isMovingSale = sale.SaleType == SaleType.Moving;
            salesDto.Validate(isMovingSale);

            var log = salesDto.ToUpdateStockLogsDto(sale, employeeId);

            sale.ToSalesEntity(salesDto);

            if (isMovingSale)
            {
                await salesRepository.UpdateAsync(sale);
                await stockLogsService.CreateStockLogsAsync(log);
                return;
            }

            var matchingProducts = (await productRepository.GetAllProductsWithTagsAsync(sale.SalesTags.ToList())).ToList();
            var matchingIds = matchingProducts.Select(p => p.Id).ToHashSet();

            foreach (var old in sale.Products.Where(p => !matchingIds.Contains(p.Id)).ToList())
                sale.Products.Remove(old);

            foreach (var product in matchingProducts)
                if (!sale.Products.Any(p => p.Id == product.Id))
                    sale.Products.Add(product);

            await salesRepository.UpdateAsync(sale);
            await stockLogsService.CreateStockLogsAsync(log);
        }
    }
}
