using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Shared.DTOs.Stock
{
    public class DeletedItemDto
    {
        public Guid Id { get; set; }
        public DeletedItemTypeDto Type { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}
