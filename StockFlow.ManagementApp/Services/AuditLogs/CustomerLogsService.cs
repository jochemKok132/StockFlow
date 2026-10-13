using StockFlow.Shared.DTOs.AuditLogs.Customer;
using StockFlow.Shared.DTOs.AuditLogs.Stock;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.AuditLogs;

namespace StockFlow.ManagementApp.Services.AuditLogs
{
    public class CustomerLogsService(IHttpService httpService) : ICustomerLogsService
    {
        public async Task<(List<CustomerLogsDto>, string error)> GetPaginatedCustomerLogsAsync(CustomerLogsPaginationDto pagination)
        {
            var url = "CustomerLogs/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&customerId={pagination.CustomerId}" +
                $"&customerName={pagination.CustomerName}" +
                $"&email={pagination.Email}" +
                $"&logType={pagination.LogType}";
                
            var response = await httpService.GetAsync<List<CustomerLogsDto>>(url);
            if (response.Succeeded)
                return (response.Value!, "");
            else
                return (new List<CustomerLogsDto>(), $"Error: {response.Message}");
        }
    }
}