using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Support;

namespace StockFlow.Application.Interfaces
{
    public interface ITicketService
    {
        Task<TicketDto> CreateAsync(CreateTicketRequest request, Guid employeeId);
        Task<List<TicketDto>> GetMineAsync(Guid employeeId);
        Task<List<TicketDto>> GetSupportRequestsAsync(TicketStatusDto? status);
        Task<List<TicketDto>> GetFeedbackAsync(TicketStatusDto? status);
        Task<TicketDto> HandleSupportRequestAsync(int ticketNumber, HandleTicketRequest request, Guid handlerId);
        Task<TicketDto> HandleFeedbackAsync(int ticketNumber, HandleTicketRequest request, Guid handlerId);
    }
}
