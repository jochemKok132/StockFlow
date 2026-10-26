using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Shared.DTOs.Ordering
{
    public class PurchaseOrderDto
    {
        public Guid Id { get; set; }
        public int OrderNumber { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public PurchaseOrderStatusDto Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDelivery { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public string? ReceivedByName { get; set; }
        public int TotalItems { get; set; }
        public List<PurchaseOrderLineDto> Lines { get; set; } = [];
    }

    public class PurchaseOrderLineDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Barcode { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public bool CanExpire { get; set; }
        public int Amount { get; set; }
        public int ReceivedAmount { get; set; }
        public OrderLineSourceDto Source { get; set; }
        public int? CustomerOrderNumber { get; set; }
    }

    public class AddOrderLineRequest
    {
        public int Barcode { get; set; }
        public int Amount { get; set; } = 1;
    }

    public class UpdateOrderLineRequest
    {
        public Guid LineId { get; set; }
        public int Amount { get; set; }
    }

    public class ReceiveOrderRequest
    {
        public List<ReceiveOrderLine> Lines { get; set; } = [];
    }

    public class ReceiveOrderLine
    {
        public Guid LineId { get; set; }
        public int ReceivedAmount { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    public class ReorderSuggestionDto
    {
        public Guid ProductId { get; set; }
        public int Barcode { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int ReservedStock { get; set; }
        public int OnOrder { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public int SuggestedAmount { get; set; }
        public bool AutoOrder { get; set; }
    }
}
