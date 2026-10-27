using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.AuditLogs;
using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.Application.Interfaces.Repositories.ManagementApp.AuditLogs
{
    public interface IReturnLogsRepository :
        IEfRepository<ReturnLogs>,
        IEfCreatableRepository<ReturnLogs>
    {
        Task<List<ReturnLogs>> GetAllReturnLogsAsync(ReturnLogsPaginationDto pagination);
    }
}
