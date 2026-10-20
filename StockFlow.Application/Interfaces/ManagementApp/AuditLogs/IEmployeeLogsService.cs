using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.AuditLogs.Employee;

namespace StockFlow.Application.Interfaces.ManagementApp.AuditLogs
{
    public interface IEmployeeLogsService
    {
        Task<List<EmployeeLogsDto>> GetAllEmployeeLogsAsync(EmployeeLogsPaginationDto pagination, bool includeTesters = false);
        Task CreateEmployeeLogsAsync(CreateEmployeeLogsDto log);
    }
}
