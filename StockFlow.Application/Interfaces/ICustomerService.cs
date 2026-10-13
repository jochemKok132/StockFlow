using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Customer.CustomerAuthentication;
using StockFlow.Shared.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<Token> LoginAsync(LoginRequest request);
        Task UpdateCustomerAsync(UpdateCustomerDto dto);
        Task ChangePasswordAsync(Guid customerId, ChangePasswordRequest request);
        Task<Token> RegisterCustomerAsync(RegisterRequest register);
        Task<CustomerDto> GetCustomerByEmailAsync(string id);
        Task<CustomerDto> GetCustomerByIdAsync(Guid id);
        Task<CustomerDto> GetCustomerByCustomerIdAsync(string customerId);
        Task<List<CustomerDto>> GetAllCustomersAsync(CustomerPaginationDto pagination);
    }
}
