using StockFlow.Shared.DTOs.AuditLogs.Customer;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.AuditLogs;
using System;
using System.Collections.Generic;
using System.Text;
using StockFlow.Application.Validations;
using StockFlow.Application.Validations.AuditLogs;

namespace StockFlow.Application.Services.ManagementApp.AuditLogs
{
    public class CustomerLogsService(ICustomerLogsRepository customerLogsRepository) : ICustomerLogsService
    {
        public async Task CreateCustomerLogsAsync(CreateCustomerLogsDto log)
        {
            log.Validate();

            await customerLogsRepository.AddAsync(log.ToCustomerLogsEntity());
        }

        public async Task<List<CustomerLogsDto>> GetAllCustomerLogsAsync(CustomerLogsPaginationDto pagination)
        {
            pagination.Validate();

            var customerLogs = await customerLogsRepository.GetAllCustomerLogsAsync(pagination);

            var result = new List<CustomerLogsDto>();
            foreach (var customerLog in customerLogs)
            {
                result.Add(customerLog.ToCustomerLogsDto());
            }
            return result;
        }

    }
}
