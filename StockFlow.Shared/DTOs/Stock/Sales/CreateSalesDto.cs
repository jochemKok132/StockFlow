using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
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
        public int PercentageOff { get; set; }
        public IEnumerable<string> SalesTags { get; set; } = [];
    }
}
