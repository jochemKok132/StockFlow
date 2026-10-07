using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.DTOs.Customer;
using StockFlow.Domain.Entities.People;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.People
{
    public static class CustomerMappings
    {
        public static CustomerDto ToCustomerDto(this Customer customer)
        {
            return new CustomerDto()
            {
                Id = customer.Id,
                Email = customer.Email,
                FullName = customer.FullName,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
                CustomerId = customer.CustomerId,
                HouseNumber = customer.HouseNumber,
                PostalCode = customer.PostalCode,
                SavedPoints = customer.SavedPoints,
                SoftDeleted = customer.SoftDeleted,
                SoftDeletedAt = customer.SoftDeletedAt,
            };
        }
        public static void ToCustomerEntity(this Customer entity, UpdateCustomerDto dto)
        {
            entity.FullName = dto.FullName;
            entity.HouseNumber = dto.HouseNumber;
            entity.PostalCode = dto.PostalCode;
            entity.Email = dto.Email;
            entity.HashedPassword = dto.HashedPassword;
            entity.SavedPoints = dto.SavedPoints;
        }
    }
}
