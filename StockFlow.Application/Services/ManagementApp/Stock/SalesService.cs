using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.Sales;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Services.ManagementApp.Stock
{
    public class SalesService(ISalesRepository salesRepository) : ISalesService
    {
        public async Task CreateSalesAsync(CreateSalesDto sale)
        {
            if (sale == null) throw new ArgumentNullException("Sale cant be null.");

            await salesRepository.AddAsync(sale.ToSalesEntity());
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

        public async Task SoftDeleteSalesAsync(Guid id)
        {
            var sale = await salesRepository.GetByIdAsync(id);
            if (sale == null) throw new KeyNotFoundException($"Sale with the id {id} not found.");

            await salesRepository.SoftDeleteAsync(sale);
        }

        public async Task UpdateSalesAsync(UpdateSalesDto salesDto)
        {
            var sale = await salesRepository.GetByIdAsync(salesDto.Id);
            if (sale == null) throw new KeyNotFoundException($"Sale with the id {salesDto.Id} not found.");

            sale.ToSalesEntity(salesDto);
            await salesRepository.UpdateAsync(sale);
        }
    }
}
