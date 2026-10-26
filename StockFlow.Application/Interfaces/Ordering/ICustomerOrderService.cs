using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Ordering;

namespace StockFlow.Application.Interfaces.Ordering
{
    public interface ICustomerOrderService
    {
        Task<List<CustomerOrderDto>> GetOrdersAsync(CustomerOrderStatusDto? status);
        Task<CustomerOrderDto> GetByNumberAsync(int orderNumber);
        Task<CustomerOrderDto> CreateAsync(CreateCustomerOrderRequest request, Guid employeeId);
        Task<CustomerOrderDto> CancelAsync(int orderNumber);
        Task<CustomerOrderDto> GetForPickupAsync(int orderNumber);
        Task ValidatePickupAsync(Purchase purchase);
        Task MarkPickedUpAsync(int orderNumber, string receiptId);
    }

    public interface IStockAllocator
    {
        Task<int> AllocateAsync(IEnumerable<Guid> productIds);
    }
}
