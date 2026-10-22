using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Shared.DTOs.Support
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public int TicketNumber { get; set; }
        public TicketTypeDto Type { get; set; }
        public TicketStatusDto Status { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Response { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public string CreatedByEmployeeId { get; set; } = string.Empty;
        public string? HandledByName { get; set; }
        public string? HandledByEmployeeId { get; set; }
        public DateTime? HandledAt { get; set; }
    }

    public class CreateTicketRequest
    {
        public TicketTypeDto Type { get; set; } = TicketTypeDto.Support;
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(FieldLimits.Description, ErrorMessage = FieldLimits.TooLongMessage)]
        public string Description { get; set; } = string.Empty;
    }

    public class HandleTicketRequest
    {
        public TicketStatusDto Status { get; set; } = TicketStatusDto.InProgress;
        [MaxLength(FieldLimits.Description, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? Response { get; set; }
    }
}
