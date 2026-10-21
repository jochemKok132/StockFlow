using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.Customer
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool SoftDeleted { get; set; }
        public DateTime SoftDeletedAt { get; set; }
        public required string CustomerId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? PostalCode { get; set; }
        public string? HouseNumber { get; set; }
        public int SavedPoints { get; set; } = 0;
        public DateTime? MovingSaleUntil { get; set; }
        public StockFlow.Shared.DTOs.Enums.MovingSaleStatusDto MovingSaleStatus { get; set; }
        public DateTime? MovingSaleRequestedAt { get; set; }
        public string? MovingSalePostalCode { get; set; }
        public string? MovingSaleHouseNumber { get; set; }
        public DateTime? MovingSaleReviewedAt { get; set; }
        public string? MovingSaleReviewedBy { get; set; }
        public StockFlow.Shared.DTOs.Stock.Sales.SalesDto? MovingSale { get; set; }
    }
}
