using StockFlow.ManagementApp.Interfaces;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Support;

namespace StockFlow.ManagementApp.Services
{
    public class TicketService(IHttpService httpService) : ITicketService
    {
        public async Task<(TicketDto? ticket, string message)> CreateAsync(CreateTicketRequest request)
        {
            var response = await httpService.PostAsync<TicketDto, CreateTicketRequest>("Ticket/Create", request);
            return response.Succeeded ? (response.Value, response.Message ?? "Sent.") : (null, response.Message ?? "The ticket could not be sent.");
        }

        public async Task<(List<TicketDto>, string error)> GetMineAsync()
        {
            var response = await httpService.GetAsync<List<TicketDto>>("Ticket/Mine");
            return response.Succeeded ? (response.Value ?? [], "") : ([], response.Message ?? "Your tickets could not be loaded.");
        }

        public async Task<(List<TicketDto>, string error)> GetQueueAsync(bool feedback, TicketStatusDto? status)
        {
            string url = (feedback ? "Ticket/Feedback" : "Ticket/Support") + (status is null ? "" : $"?status={status}");
            var response = await httpService.GetAsync<List<TicketDto>>(url);
            return response.Succeeded ? (response.Value ?? [], "") : ([], response.Message ?? "The tickets could not be loaded.");
        }

        public async Task<(TicketDto? ticket, string message)> HandleAsync(bool feedback, int ticketNumber, HandleTicketRequest request)
        {
            string url = $"Ticket/{(feedback ? "Feedback" : "Support")}/{ticketNumber}";
            var response = await httpService.PutAsync<TicketDto, HandleTicketRequest>(url, request);
            return response.Succeeded ? (response.Value, response.Message ?? "Saved.") : (null, response.Message ?? "The ticket could not be saved.");
        }
    }
}
