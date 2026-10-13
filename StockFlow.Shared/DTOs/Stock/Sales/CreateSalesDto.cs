using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Sales
{
    public class CreateSalesDto
    {
        public string SaleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PercentageOff { get; set; }
        public IEnumerable<string> SalesTags { get; set; } = [];
    }
}
