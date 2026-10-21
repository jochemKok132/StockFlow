using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Sales
{
    public class CreateSalesDto
    {
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public string SaleName { get; set; } = string.Empty;
        [MaxLength(FieldLimits.Description, ErrorMessage = FieldLimits.TooLongMessage)]
        public string Description { get; set; } = string.Empty;
        public SaleTypeDto SaleType { get; set; } = SaleTypeDto.Percentage;
        public int PercentageOff { get; set; }
        public int BuyAmount { get; set; }
        public int FreeAmount { get; set; }
        public decimal BundlePrice { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<string> SalesTags { get; set; } = [];
    }
}
