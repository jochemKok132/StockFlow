using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Ordering;

namespace StockFlow.Application.Interfaces.Ordering
{
    public interface IStockCountService
    {
        Task<List<StockCountDto>> GetCountsAsync(StockCountStatusDto? status);
        Task<StockCountDto> GetByNumberAsync(int countNumber);
        Task<StockCountDto> CreateAsync(CreateStockCountRequest request, Guid employeeId);
        Task<StockCountDto> CompleteAsync(int countNumber, CompleteStockCountRequest request, Guid employeeId);
        Task<StockCountDto> CancelAsync(int countNumber);
        Task<int> CreateExpiryCountsAsync(DateTime today);
    }

    public interface IStockAutomationService
    {
        Task<StockAutomationResult> RunAsync(DateTime today);
    }

    public record StockAutomationResult(int PricesApplied, int ExpiryCountsCreated, int AutomaticOrderLines, int OrdersSent);
}
