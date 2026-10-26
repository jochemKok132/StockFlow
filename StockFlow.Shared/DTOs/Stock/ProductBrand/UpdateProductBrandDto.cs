using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Stock.ProductBrand
{
    public class UpdateProductBrandDto
    {
        public Guid Id { get; set; }
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public required string BrandName { get; set; }
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
