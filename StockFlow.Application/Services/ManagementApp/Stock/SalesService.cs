using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.Sales;
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
    public class SalesService(ISalesRepository salesRepository, IProductRepository productRepository, IStockLogsService stockLogsService) : ISalesService
    {

        public async Task CreateSalesAsync(CreateSalesDto sale, Guid employeeId)
        {
            if (sale == null) throw new ArgumentNullException("Sale cant be null.");

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
            var sales = await salesRepository.GetAllSalesAsync(pagination);

            var result = new List<SalesDto>();
            foreach (var sale in sales)
            {
                result.Add(sale.ToSalesDto());
            }
            return result;
        }

        public async Task SoftDeleteSalesAsync(Guid id, Guid employeeId)
        {
            var sale = await salesRepository.GetByIdAsync(id);
            if (sale == null) throw new KeyNotFoundException($"Sale with the id {id} not found.");
            await salesRepository.SoftDeleteAsync(sale);
            await stockLogsService.CreateStockLogsAsync(sale.ToSoftDeleteStockLogsDto(employeeId));
        }

        public async Task UpdateSalesAsync(UpdateSalesDto salesDto, Guid employeeId)
        {
            var sale = await salesRepository.GetByIdWithProductsAsync(salesDto.Id);
            if (sale == null) throw new KeyNotFoundException($"Sale with the id {salesDto.Id} not found.");

            var log = salesDto.ToUpdateStockLogsDto(sale, employeeId);

            sale.ToSalesEntity(salesDto);

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
