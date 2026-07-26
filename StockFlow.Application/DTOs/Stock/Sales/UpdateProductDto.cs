using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Stock.Sales
{
    public class UpdateSalesDto
    {
        public Guid Id { get; set; }
        public required string SaleName { get; set; }
        public required string Description { get; set; }
        public int PercentageOff { get; set; }
    }
}
