namespace StockFlow.Shared.DTOs.Enums
{
    public enum TicketTypeDto
    {
        Support,
        Bug,
        Feature,
    }

    public enum TicketStatusDto
    {
        Open,
        InProgress,
        Done,
        Rejected,
    }

    public enum DeletedItemTypeDto
    {
        Product,
        ProductBrand,
        Sale,
        Shelf,
    }
}
