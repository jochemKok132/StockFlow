using StockFlow.Domain.Enums;
using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Application.Mappings.Enums
{
    public static class OrderingEnumMappings
    {
        public static ProductStatus ToDomain(this ProductStatusDto value) => Map<ProductStatusDto, ProductStatus>(value);
        public static ProductStatusDto ToDto(this ProductStatus value) => Map<ProductStatus, ProductStatusDto>(value);
        public static PurchaseOrderStatus ToDomain(this PurchaseOrderStatusDto value) => Map<PurchaseOrderStatusDto, PurchaseOrderStatus>(value);
        public static PurchaseOrderStatusDto ToDto(this PurchaseOrderStatus value) => Map<PurchaseOrderStatus, PurchaseOrderStatusDto>(value);
        public static OrderLineSourceDto ToDto(this OrderLineSource value) => Map<OrderLineSource, OrderLineSourceDto>(value);
        public static CustomerOrderStatus ToDomain(this CustomerOrderStatusDto value) => Map<CustomerOrderStatusDto, CustomerOrderStatus>(value);
        public static CustomerOrderStatusDto ToDto(this CustomerOrderStatus value) => Map<CustomerOrderStatus, CustomerOrderStatusDto>(value);
        public static CustomerOrderSourceDto ToDto(this CustomerOrderSource value) => Map<CustomerOrderSource, CustomerOrderSourceDto>(value);
        public static StockCountTypeDto ToDto(this StockCountType value) => Map<StockCountType, StockCountTypeDto>(value);
        public static StockCountStatus ToDomain(this StockCountStatusDto value) => Map<StockCountStatusDto, StockCountStatus>(value);
        public static StockCountStatusDto ToDto(this StockCountStatus value) => Map<StockCountStatus, StockCountStatusDto>(value);

        private static TOut Map<TIn, TOut>(TIn value) where TIn : struct, Enum where TOut : struct, Enum =>
            Enum.IsDefined(value) && Enum.TryParse<TOut>(value.ToString(), out var result) && Enum.IsDefined(result)
                ? result
                : throw new ArgumentOutOfRangeException(nameof(value), $"Unknown {typeof(TIn).Name} value {value}.");
    }
}
