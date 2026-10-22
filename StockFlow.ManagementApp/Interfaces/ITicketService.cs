using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Support;

namespace StockFlow.ManagementApp.Interfaces
{
    public interface ITicketService
    {
        Task<(TicketDto? ticket, string message)> CreateAsync(CreateTicketRequest request);
        Task<(List<TicketDto>, string error)> GetMineAsync();
        Task<(List<TicketDto>, string error)> GetQueueAsync(bool feedback, TicketStatusDto? status);
        Task<(TicketDto? ticket, string message)> HandleAsync(bool feedback, int ticketNumber, HandleTicketRequest request);
    }
}
