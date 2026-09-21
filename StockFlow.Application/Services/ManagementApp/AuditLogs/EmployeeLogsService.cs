using StockFlow.Application.DTOs.AuditLogs.Employee;
using StockFlow.Application.DTOs.AuditLogs.Stock;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Services.ManagementApp.AuditLogs
{
    public class EmployeeLogsService(IEmployeeLogsRepository EmployeeLogsRepository) : IEmployeeLogsService
    {
        public async Task CreateEmployeeLogsAsync(CreateEmployeeLogsDto log)
        {
            if (log == null) throw new ArgumentNullException("EmployeeLog cant be null.");

            await EmployeeLogsRepository.AddAsync(log.ToEmployeeLogsEntity());
        }

        public async Task<List<EmployeeLogsDto>> GetAllEmployeeLogsAsync(EmployeeLogsPaginationDto pagination)
        {
            var EmployeeLogs = await EmployeeLogsRepository.GetAllEmployeeLogsAsync(pagination);

            var result = new List<EmployeeLogsDto>();
            foreach (var EmployeeLog in EmployeeLogs)
            {
                result.Add(EmployeeLog.ToEmployeeLogsDto());
            }
            return result;
        }

    }
}
