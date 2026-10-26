using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock.Product;

namespace StockFlow.Shared.DTOs.Ordering
{
    public class CustomerOrderDto
    {
        public Guid Id { get; set; }
        public int OrderNumber { get; set; }
        public CustomerOrderStatusDto Status { get; set; }
        public CustomerOrderSourceDto Source { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public CustomerDto? Customer { get; set; }
        public string? Note { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadyAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? ReceiptId { get; set; }
        public List<CustomerOrderLineDto> Lines { get; set; } = [];
    }

    public class CustomerOrderLineDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Barcode { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Amount { get; set; }
        public int ReservedAmount { get; set; }
        public ProductDto? Product { get; set; }
    }

    public class CreateCustomerOrderRequest
    {
        [MaxLength(IdentifierFormats.CustomerIdLength, ErrorMessage = FieldLimits.TooLongMessage)]
        public string CustomerId { get; set; } = string.Empty;
        [MaxLength(FieldLimits.OrderNote, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? Note { get; set; }
        public List<CustomerOrderLineRequest> Lines { get; set; } = [];
    }

    public class CustomerOrderLineRequest
    {
        public int Barcode { get; set; }
        public int Amount { get; set; } = 1;
    }
}
