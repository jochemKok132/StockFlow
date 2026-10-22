using StockFlow.Domain.Entities.IEntities;
using StockFlow.Domain.Entities.People;
using StockFlow.Domain.Enums;

namespace StockFlow.Domain.Entities.Support
{
    public class Ticket : IEntity, ICreateable, IUpdatable
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TicketNumber { get; set; }
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Response { get; set; }
        public Guid CreatedById { get; set; }
        public Employee CreatedBy { get; set; } = null!;
        public Guid? HandledById { get; set; }
        public Employee? HandledBy { get; set; }
        public DateTime? HandledAt { get; set; }
    }
}
