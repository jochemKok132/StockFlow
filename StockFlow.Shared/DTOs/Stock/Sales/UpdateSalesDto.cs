using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;
using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.Sales
{
    public class UpdateSalesDto
    {
        public Guid Id { get; set; }
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public required string SaleName { get; set; }
        [MaxLength(FieldLimits.Description, ErrorMessage = FieldLimits.TooLongMessage)]
        public required string Description { get; set; }
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
