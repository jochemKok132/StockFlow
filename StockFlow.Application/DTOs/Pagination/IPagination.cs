using StockFlow.Application.DTOs.Enums;

namespace StockFlow.Application.DTOs.Pagination
{
    public interface IPagination
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public OrderType OrderBy { get; set; }
        public OrderByType OrderByType { get; set; }
    }
}
