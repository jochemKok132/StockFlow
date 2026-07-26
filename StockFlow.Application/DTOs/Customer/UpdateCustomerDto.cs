using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.Customer
{
    public class UpdateCustomerDto
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? PostalCode { get; set; }
        public string? HouseNumber { get; set; }
        public required string HashedPassword { get; set; }
        public int SavedPoints { get; set; } = 0;
    }
}
