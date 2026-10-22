using StockFlow.Domain.Entities.Support;
using StockFlow.Domain.Enums;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Support;

namespace StockFlow.Application.Mappings.Enums
{
    public static class TicketMappings
    {
        public static TicketType ToDomain(this TicketTypeDto type) => type switch
        {
            TicketTypeDto.Support => TicketType.Support,
            TicketTypeDto.Bug => TicketType.Bug,
            TicketTypeDto.Feature => TicketType.Feature,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        public static TicketTypeDto ToDto(this TicketType type) => type switch
        {
            TicketType.Support => TicketTypeDto.Support,
            TicketType.Bug => TicketTypeDto.Bug,
            TicketType.Feature => TicketTypeDto.Feature,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        public static TicketStatus ToDomain(this TicketStatusDto status) => status switch
        {
            TicketStatusDto.Open => TicketStatus.Open,
            TicketStatusDto.InProgress => TicketStatus.InProgress,
            TicketStatusDto.Done => TicketStatus.Done,
            TicketStatusDto.Rejected => TicketStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };

        public static TicketStatusDto ToDto(this TicketStatus status) => status switch
        {
            TicketStatus.Open => TicketStatusDto.Open,
            TicketStatus.InProgress => TicketStatusDto.InProgress,
            TicketStatus.Done => TicketStatusDto.Done,
            TicketStatus.Rejected => TicketStatusDto.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };

        public static TicketDto ToTicketDto(this Ticket ticket) => new()
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            Type = ticket.Type.ToDto(),
            Status = ticket.Status.ToDto(),
            Title = ticket.Title,
            Description = ticket.Description,
            Response = ticket.Response,
            CreatedAt = ticket.CreatedAt,
            CreatedByName = ticket.CreatedBy?.FullName ?? "-",
            CreatedByEmployeeId = ticket.CreatedBy?.EmployeeId ?? "-",
            HandledByName = ticket.HandledBy?.FullName,
            HandledByEmployeeId = ticket.HandledBy?.EmployeeId,
            HandledAt = ticket.HandledAt,
        };
    }
}
