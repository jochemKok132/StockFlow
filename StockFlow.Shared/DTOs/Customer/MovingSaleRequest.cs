using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;

namespace StockFlow.Shared.DTOs.Customer
{
    public class MovingSaleRequest
    {
        [MaxLength(FieldLimits.PostalCode, ErrorMessage = FieldLimits.TooLongMessage)]
        public string PostalCode { get; set; } = string.Empty;
        [MaxLength(FieldLimits.HouseNumber, ErrorMessage = FieldLimits.TooLongMessage)]
        public string HouseNumber { get; set; } = string.Empty;
    }
}
