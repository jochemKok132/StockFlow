using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Interfaces;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Support;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketController(ITicketService ticketService) : ControllerBase
    {
        [Authorize(Roles = Permissions.Employees)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(CreateTicketRequest request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<TicketDto>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                TicketDto ticket = await ticketService.CreateAsync(request, userId);
                return Ok(ApiResponse<TicketDto>.Ok(ticket, $"Ticket {ticket.TicketNumber} sent."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<TicketDto>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.Employees)]
        [HttpGet("Mine")]
        public async Task<IActionResult> GetMineAsync()
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<List<TicketDto>>.Fail(ApiMessages.NotLoggedIn));

            return Ok(ApiResponse<List<TicketDto>>.Ok(await ticketService.GetMineAsync(userId)));
        }

        [Authorize(Roles = Permissions.ViewSupport)]
        [HttpGet("Support")]
        public Task<IActionResult> GetSupportRequestsAsync([FromQuery] TicketStatusDto? status) =>
            ListAsync(() => ticketService.GetSupportRequestsAsync(status));

        [Authorize(Roles = Permissions.ViewFeedback)]
        [HttpGet("Feedback")]
        public Task<IActionResult> GetFeedbackAsync([FromQuery] TicketStatusDto? status) =>
            ListAsync(() => ticketService.GetFeedbackAsync(status));

        [Authorize(Roles = Permissions.HandleSupport)]
        [HttpPut("Support/{ticketNumber:int}")]
        public Task<IActionResult> HandleSupportRequestAsync(int ticketNumber, HandleTicketRequest request) =>
            HandleAsync(userId => ticketService.HandleSupportRequestAsync(ticketNumber, request, userId));

        [Authorize(Roles = Permissions.HandleFeedback)]
        [HttpPut("Feedback/{ticketNumber:int}")]
        public Task<IActionResult> HandleFeedbackAsync(int ticketNumber, HandleTicketRequest request) =>
            HandleAsync(userId => ticketService.HandleFeedbackAsync(ticketNumber, request, userId));

        private async Task<IActionResult> ListAsync(Func<Task<List<TicketDto>>> list)
        {
            try
            {
                return Ok(ApiResponse<List<TicketDto>>.Ok(await list()));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<TicketDto>>.Fail(exception.Message));
            }
        }

        private async Task<IActionResult> HandleAsync(Func<Guid, Task<TicketDto>> handle)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<TicketDto>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                TicketDto ticket = await handle(userId);
                return Ok(ApiResponse<TicketDto>.Ok(ticket, $"Ticket {ticket.TicketNumber} saved."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<TicketDto>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<TicketDto>.Fail(exception.Message));
            }
        }
    }
}
