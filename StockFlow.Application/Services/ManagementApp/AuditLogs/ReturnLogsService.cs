using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.AuditLogs;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Validations;
using StockFlow.Shared.DTOs.AuditLogs.Return;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.Application.Services.ManagementApp.AuditLogs
{
    public class ReturnLogsService(IReturnLogsRepository returnLogsRepository) : IReturnLogsService
    {
        public async Task<List<ReturnLogDto>> GetAllReturnLogsAsync(ReturnLogsPaginationDto pagination)
        {
            pagination.Validate();
            if (pagination.OrderBy is var orderBy && !Enum.IsDefined(orderBy))
                throw new ArgumentException("Order by is invalid.");
            var logs = await returnLogsRepository.GetAllReturnLogsAsync(pagination);
            return logs.Select(l => l.ToReturnLogDto()).ToList();
        }
    }
}
