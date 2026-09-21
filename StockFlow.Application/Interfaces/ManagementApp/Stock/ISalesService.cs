using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.Sales;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces.ManagementApp.Stock
{
    public interface ISalesService
    {
        Task<List<SalesDto>> GetAllSalesAsync(SalesPaginationDto pagination);
        Task CreateSalesAsync(CreateSalesDto product, Guid employeeId);
        Task UpdateSalesAsync(UpdateSalesDto product, Guid employeeId);
        Task SoftDeleteSalesAsync(Guid id, Guid employeeId);
    }
}
