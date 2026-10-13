using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Shared.DTOs.AuditLogs.Customer;

namespace StockFlow.Application.Interfaces.ManagementApp.AuditLogs
{
    public interface ICustomerLogsService
    {
        Task<List<CustomerLogsDto>> GetAllCustomerLogsAsync(CustomerLogsPaginationDto pagination);
        Task CreateCustomerLogsAsync(CreateCustomerLogsDto log);
    }
}
