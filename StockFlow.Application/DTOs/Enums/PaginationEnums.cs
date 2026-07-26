using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application.DTOs.Enums
{
    public enum OrderType
    {
        Descending,
        Ascending
    }
    public enum OrderByType
    {
        SearchTerm,
        CreatedAt,
        UpdatedAt,
        Priority,
        LeadTime
    }
}
