using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Shared.DTOs.Stock.Product
{
    public class WriteOffRequest
    {
        public Guid ProductId { get; set; }
        public int Amount { get; set; } = 1;
        public WriteOffReasonDto Reason { get; set; } = WriteOffReasonDto.Damaged;
        [MaxLength(FieldLimits.WriteOffNote, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? Note { get; set; }
    }
}
