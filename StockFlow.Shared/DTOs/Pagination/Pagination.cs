using StockFlow.Shared.DTOs.Enums;
using System;

namespace StockFlow.Shared.DTOs.Pagination
{
    public abstract class PaginationBase
    {
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
        public OrderType OrderType { get; set; }
    }

    public class Pagination<TOrderBy> : PaginationBase where TOrderBy : Enum
    {
        public TOrderBy OrderBy { get; set; } = default!;
    }
}