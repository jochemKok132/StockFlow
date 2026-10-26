namespace StockFlow.Shared.DTOs.Enums
{
    public enum ProductStatusDto
    {
        Active,
        Draft,
        Disabled,
    }

    public enum PurchaseOrderStatusDto
    {
        Open,
        Sent,
        Received,
        Cancelled,
    }

    public enum OrderLineSourceDto
    {
        Manual,
        Automatic,
        CustomerOrder,
    }

    public enum CustomerOrderStatusDto
    {
        Waiting,
        ReadyForPickup,
        PickedUp,
        Cancelled,
    }

    public enum CustomerOrderSourceDto
    {
        Manual,
        Online,
    }

    public enum StockCountTypeDto
    {
        Manual,
        Expiry,
    }

    public enum StockCountStatusDto
    {
        Open,
        Completed,
        Cancelled,
    }

    public enum SupplierOrderBy
    {
        SupplierName,
        OrderDay,
        CreatedAt,
    }
}
