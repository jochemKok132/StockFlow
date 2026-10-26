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
        [MaxLength(FieldLimits.Description, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? Description { get; set; }
        [MaxLength(FieldLimits.Url, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? Website { get; set; }
        [MaxLength(FieldLimits.Country, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? Country { get; set; }
        [MaxLength(FieldLimits.Email, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? ContactEmail { get; set; }
        [MaxLength(FieldLimits.Phone, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? CustomerServicePhone { get; set; }
        public byte[]? Logo { get; set; }
    }
}
