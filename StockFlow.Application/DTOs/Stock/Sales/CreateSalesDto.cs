using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Stock.Sales
{
    public class CreateSalesDto
    {
        public required string SaleName { get; set; }
        public required string Description { get; set; }
        public int PercentageOff { get; set; }
        public IEnumerable<string> SalesTags { get; set; } = [];
    }
}
