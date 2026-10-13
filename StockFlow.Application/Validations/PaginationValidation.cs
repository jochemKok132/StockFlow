using StockFlow.Shared.DTOs.Pagination;

namespace StockFlow.Application.Validations
{
    public static class PaginationValidation
    {
        public const int MaxPageSize = 100;

        public static void Validate(this PaginationBase pagination)
        {
            Guard.NotNull(pagination, "Pagination");

            if (pagination.PageNumber < 1)
                throw new ArgumentException("Page number must be 1 or higher.");

            if (pagination.PageSize < 1 || pagination.PageSize > MaxPageSize)
                throw new ArgumentException($"Page size must be between 1 and {MaxPageSize}.");

            if (!Enum.IsDefined(pagination.OrderType))
                throw new ArgumentException("Order type is invalid.");
        }
    }
}
