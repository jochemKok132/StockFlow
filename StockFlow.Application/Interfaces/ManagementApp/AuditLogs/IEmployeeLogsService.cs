using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.AuditLogs.Employee;

namespace StockFlow.Application.Interfaces.ManagementApp.AuditLogs
{
    public interface IEmployeeLogsService
    {
        Task<List<EmployeeLogsDto>> GetAllEmployeeLogsAsync(EmployeeLogsPaginationDto pagination);
        Task CreateEmployeeLogsAsync(CreateEmployeeLogsDto log);
    }
}
