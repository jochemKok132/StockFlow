using StockFlow.Application.Interfaces;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Application.Mappings.Enums;
using StockFlow.Application.Validations.Support;
using StockFlow.Domain.Entities.Support;
using StockFlow.Domain.Enums;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Support;

namespace StockFlow.Application.Services.ManagementApp
{
    public class TicketService(ITicketRepository ticketRepository) : ITicketService
    {
        public const string NotASupportRequest = "This ticket is not a support request.";
        public const string NotFeedback = "This ticket is not a bug report or feature request.";

        private static readonly TicketType[] SupportTypes = [TicketType.Support];
        private static readonly TicketType[] FeedbackTypes = [TicketType.Bug, TicketType.Feature];

        public async Task<TicketDto> CreateAsync(CreateTicketRequest request, Guid employeeId)
        {
            request.Validate();

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                TicketNumber = await ticketRepository.GetNextTicketNumberAsync(),
                Type = request.Type.ToDomain(),
                Status = TicketStatus.Open,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                CreatedById = employeeId,
            };

            await ticketRepository.AddAsync(ticket);
            return ticket.ToTicketDto();
        }

        public async Task<List<TicketDto>> GetMineAsync(Guid employeeId) =>
            (await ticketRepository.GetForEmployeeAsync(employeeId)).Select(t => t.ToTicketDto()).ToList();

        public Task<List<TicketDto>> GetSupportRequestsAsync(TicketStatusDto? status) => GetAsync(SupportTypes, status);

        public Task<List<TicketDto>> GetFeedbackAsync(TicketStatusDto? status) => GetAsync(FeedbackTypes, status);

        public Task<TicketDto> HandleSupportRequestAsync(int ticketNumber, HandleTicketRequest request, Guid handlerId) =>
            HandleAsync(ticketNumber, request, handlerId, SupportTypes, NotASupportRequest);

        public Task<TicketDto> HandleFeedbackAsync(int ticketNumber, HandleTicketRequest request, Guid handlerId) =>
            HandleAsync(ticketNumber, request, handlerId, FeedbackTypes, NotFeedback);

        private async Task<List<TicketDto>> GetAsync(TicketType[] types, TicketStatusDto? status)
        {
            if (status is { } value && !Enum.IsDefined(value))
                throw new ArgumentException("Choose a valid status.");

            var tickets = await ticketRepository.GetByTypesAsync(types, status?.ToDomain());
            return tickets.Select(t => t.ToTicketDto()).ToList();
        }

        private async Task<TicketDto> HandleAsync(int ticketNumber, HandleTicketRequest request, Guid handlerId, TicketType[] types, string wrongType)
        {
            request.Validate();

            var ticket = await ticketRepository.GetByNumberAsync(ticketNumber)
                ?? throw new KeyNotFoundException($"Ticket {ticketNumber} not found.");

            if (!types.Contains(ticket.Type))
                throw new ArgumentException(wrongType);

            ticket.Status = request.Status.ToDomain();
            ticket.Response = string.IsNullOrWhiteSpace(request.Response) ? null : request.Response.Trim();
            ticket.HandledById = handlerId;
            ticket.HandledAt = DateTime.UtcNow;

            await ticketRepository.UpdateAsync(ticket);

            var updated = await ticketRepository.GetByNumberAsync(ticketNumber) ?? ticket;
            return updated.ToTicketDto();
        }
    }
}
