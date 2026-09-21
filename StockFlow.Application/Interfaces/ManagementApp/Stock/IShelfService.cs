using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.Shelf;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.ManagementApp.Stock
{
    public interface IShelfService
    {
        Task<List<ShelfDto>> GetAllShelvesAsync(ShelfPaginationDto pagination);
        Task CreateShelfAsync(CreateShelfDto product, Guid employeeId);
        Task UpdateShelfAsync(UpdateShelfDto product, Guid employeeId);
        Task SoftDeleteShelfAsync(Guid id, Guid employeeId);
    }
}
