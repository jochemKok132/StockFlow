using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.ProductBrand
{
    public class CreateProductBrandDto
    {
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public string BrandName { get; set; } = string.Empty;
    }
}
