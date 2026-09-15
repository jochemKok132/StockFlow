using StockFlow.Application.DTOs.Customer;
using StockFlow.Application.DTOs.Customer.CustomerAuthentication;
using StockFlow.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<Token> LoginAsync(LoginRequest request);
        Task UpdateCustomerAsync(UpdateCustomerDto dto);
        Task RegisterCustomerAsync(RegisterRequest register);
        Task<CustomerDto> GetCustomerByEmailAsync(string id);
        Task<CustomerDto> GetCustomerByIdAsync(Guid id);
        Task<List<CustomerDto>> GetAllCustomersAsync(CustomerPaginationDto pagination);
    }
}
