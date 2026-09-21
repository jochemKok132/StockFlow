using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.Sales;
using StockFlow.Application.DTOs.Stock.Shelf;
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
    public class ShelfService(IShelfRepository shelfRepository, IStockLogsService stockLogsService) : IShelfService
    {
        public async Task CreateShelfAsync(CreateShelfDto shelf, Guid employeeId)
        {
            if (shelf == null) throw new ArgumentNullException("Shelf cant be null.");
            await stockLogsService.CreateStockLogsAsync(shelf.ToCreateStockLogsDto(employeeId));
            await shelfRepository.AddAsync(shelf.ToShelfEntity());
        }

        public async Task<List<ShelfDto>> GetAllShelvesAsync(ShelfPaginationDto pagination)
        {
            var shelves = await shelfRepository.GetAllShelvesAsync(pagination);

            var result = new List<ShelfDto>();
            foreach (var shelf in shelves)
            {
                result.Add(shelf.ToShelfDto());
            }
            return result;
        }

        public async Task SoftDeleteShelfAsync(Guid id, Guid employeeId)
        {
            var shelf = await shelfRepository.GetByIdAsync(id);
            if (shelf == null) throw new KeyNotFoundException($"Shelf with the id {id} not found.");
            await stockLogsService.CreateStockLogsAsync(shelf.ToSoftDeleteStockLogsDto(employeeId));

            await shelfRepository.SoftDeleteAsync(shelf);
        }

        public async Task UpdateShelfAsync(UpdateShelfDto shelfDto, Guid employeeId)
        {
            var shelf = await shelfRepository.GetByIdAsync(shelfDto.Id);
            if (shelf == null) throw new KeyNotFoundException($"Shelf with the id {shelfDto.Id} not found.");
            var log = shelfDto.ToUpdateStockLogsDto(shelf, employeeId);

            shelf.ToShelfEntity(shelfDto);
            await stockLogsService.CreateStockLogsAsync(log);
            await shelfRepository.UpdateAsync(shelf);
        }
    }
}
