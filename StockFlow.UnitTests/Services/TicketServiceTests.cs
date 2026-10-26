using StockFlow.Application.Services.ManagementApp;
using StockFlow.Domain.Entities.Support;
using StockFlow.Shared.DTOs.Support;
using StockFlow.UnitTests.Helpers;

namespace StockFlow.UnitTests.Services
{
    public class TicketServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepository = new();
        private readonly TicketService _sut;

        public TicketServiceTests()
        {
            _sut = new TicketService(_ticketRepository.Object);
        }

        private static Ticket Ticket(TicketType type, int number = 1001) => new()
        {
            Id = Guid.NewGuid(),
            TicketNumber = number,
            Type = type,
            Title = "Cash register won't start",
            Description = "Error on startup",
            CreatedById = Guid.NewGuid(),
            CreatedBy = TestData.Employee(),
        };

        [Fact]
        public async Task CreateAsync_ValidRequest_AddsOpenTicketWithNextNumber()
        {
            Ticket? created = null;
            var employeeId = Guid.NewGuid();
            _ticketRepository.Setup(r => r.GetNextTicketNumberAsync()).ReturnsAsync(1005);
            _ticketRepository.Setup(r => r.AddAsync(It.IsAny<Ticket>())).Callback<Ticket>(t => created = t);

            var result = await _sut.CreateAsync(new CreateTicketRequest { Type = TicketTypeDto.Bug, Title = "  Button does not work ", Description = "Save does nothing" }, employeeId);

            Assert.NotNull(created);
            Assert.Equal(1005, created.TicketNumber);
            Assert.Equal(TicketType.Bug, created.Type);
            Assert.Equal(TicketStatus.Open, created.Status);
            Assert.Equal("Button does not work", created.Title);
            Assert.Equal(employeeId, created.CreatedById);
            Assert.Equal(1005, result.TicketNumber);
        }

        [Theory]
        [InlineData("", "x")]
        [InlineData("x", "")]
        public async Task CreateAsync_MissingText_ThrowsArgumentException(string title, string description)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(new CreateTicketRequest { Title = title, Description = description }, Guid.NewGuid()));
            _ticketRepository.Verify(r => r.AddAsync(It.IsAny<Ticket>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_UnknownType_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(new CreateTicketRequest { Type = (TicketTypeDto)9, Title = "x", Description = "y" }, Guid.NewGuid()));
        }

        [Fact]
        public async Task GetSupportRequestsAsync_AsksOnlyForSupportTickets()
        {
            IReadOnlyCollection<TicketType>? asked = null;
            _ticketRepository.Setup(r => r.GetByTypesAsync(It.IsAny<IReadOnlyCollection<TicketType>>(), TicketStatus.Open))
                .Callback<IReadOnlyCollection<TicketType>, TicketStatus?>((types, _) => asked = types)
                .ReturnsAsync(new[] { Ticket(TicketType.Support) });

            var result = await _sut.GetSupportRequestsAsync(TicketStatusDto.Open);

            Assert.Equal([TicketType.Support], asked);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetFeedbackAsync_AsksForBugsAndFeatures()
        {
            IReadOnlyCollection<TicketType>? asked = null;
            _ticketRepository.Setup(r => r.GetByTypesAsync(It.IsAny<IReadOnlyCollection<TicketType>>(), null))
                .Callback<IReadOnlyCollection<TicketType>, TicketStatus?>((types, _) => asked = types)
                .ReturnsAsync(Array.Empty<Ticket>());

            await _sut.GetFeedbackAsync(null);

            Assert.Equal([TicketType.Bug, TicketType.Feature], asked);
        }

        [Fact]
        public async Task HandleSupportRequestAsync_SupportTicket_SavesStatusAnswerAndHandler()
        {
            var ticket = Ticket(TicketType.Support);
            var handlerId = Guid.NewGuid();
            _ticketRepository.Setup(r => r.GetByNumberAsync(1001)).ReturnsAsync(ticket);

            var result = await _sut.HandleSupportRequestAsync(1001, new HandleTicketRequest { Status = TicketStatusDto.Done, Response = " Restarted " }, handlerId);

            Assert.Equal(TicketStatus.Done, ticket.Status);
            Assert.Equal("Restarted", ticket.Response);
            Assert.Equal(handlerId, ticket.HandledById);
            Assert.NotNull(ticket.HandledAt);
            Assert.Equal(TicketStatusDto.Done, result.Status);
            _ticketRepository.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        [Fact]
        public async Task HandleSupportRequestAsync_BugTicket_ThrowsArgumentException()
        {
            _ticketRepository.Setup(r => r.GetByNumberAsync(1001)).ReturnsAsync(Ticket(TicketType.Bug));

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.HandleSupportRequestAsync(1001, new HandleTicketRequest(), Guid.NewGuid()));

            Assert.Equal(TicketService.NotASupportRequest, exception.Message);
            _ticketRepository.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        [Fact]
        public async Task HandleFeedbackAsync_SupportTicket_ThrowsArgumentException()
        {
            _ticketRepository.Setup(r => r.GetByNumberAsync(1001)).ReturnsAsync(Ticket(TicketType.Support));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.HandleFeedbackAsync(1001, new HandleTicketRequest(), Guid.NewGuid()));
        }

        [Fact]
        public async Task HandleFeedbackAsync_UnknownTicket_ThrowsKeyNotFoundException()
        {
            _ticketRepository.Setup(r => r.GetByNumberAsync(It.IsAny<int>())).ReturnsAsync((Ticket?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.HandleFeedbackAsync(9999, new HandleTicketRequest(), Guid.NewGuid()));
        }

        [Fact]
        public async Task HandleFeedbackAsync_EmptyAnswer_StoresNull()
        {
            var ticket = Ticket(TicketType.Feature);
            ticket.Response = "original";
            _ticketRepository.Setup(r => r.GetByNumberAsync(1001)).ReturnsAsync(ticket);

            await _sut.HandleFeedbackAsync(1001, new HandleTicketRequest { Status = TicketStatusDto.Rejected, Response = "   " }, Guid.NewGuid());

            Assert.Null(ticket.Response);
            Assert.Equal(TicketStatus.Rejected, ticket.Status);
        }
    }
}
