using System.ComponentModel.DataAnnotations;
using StockFlow.Shared.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Customer
{
    public class UpdateCustomerDto
    {
        public Guid Id { get; set; }
        [MaxLength(FieldLimits.Name, ErrorMessage = FieldLimits.TooLongMessage)]
        public required string FullName { get; set; }
        [MaxLength(FieldLimits.Email, ErrorMessage = FieldLimits.TooLongMessage)]
        public required string Email { get; set; }
        [MaxLength(FieldLimits.PostalCode, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? PostalCode { get; set; }
        [MaxLength(FieldLimits.HouseNumber, ErrorMessage = FieldLimits.TooLongMessage)]
        public string? HouseNumber { get; set; }
        public int SavedPoints { get; set; } = 0;
    }
}
