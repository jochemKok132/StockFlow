using StockFlow.Application.DTOs.Customer;
using StockFlow.Application.DTOs.Customer.CustomerAuthentication;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces;
using StockFlow.Application.Interfaces.Helpers;
using StockFlow.Application.Interfaces.ManagementApp;
using StockFlow.Application.Interfaces.Repositories.ManagementApp;
using StockFlow.Application.Mappings.People;
using StockFlow.Domain.Entities.People;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace StockFlow.Application.Services.ManagementApp
{
    public class CustomerService(ICustomerRepository customerRepository, IAuthenticationService authenticationService, IPassword password) : ICustomerService
    {
        public async Task<List<CustomerDto>> GetAllCustomersAsync(CustomerPaginationDto pagination)
        {
            var customers = await customerRepository.GetAllCustomersAsync(pagination);
            return customers.Select(e => e.ToCustomerDto()).ToList();
        }

        public async Task<CustomerDto> GetCustomerByEmailAsync(string email)
        {
            var customer = await customerRepository.GetCustomerByEmailAsync(email);
            return customer.ToCustomerDto();
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(Guid id)
        {
            var customer = await customerRepository.GetByIdAsync(id);
            return customer.ToCustomerDto();
        }

        public async Task<Token> LoginAsync(LoginRequest request)
        {
            Customer? customer = await customerRepository.GetCustomerByEmailAsync(request.Email);

            if (customer == null)
                throw new KeyNotFoundException("The Customer with this id cannot be found.");

            if(!password.Validate(customer.HashedPassword, request.Password))
                throw new AuthenticationException("The password is incorrect.");

            return authenticationService.GenerateToken(customer);
        }

        public Task RegisterCustomerAsync(RegisterRequest register)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCustomerAsync(UpdateCustomerDto dto)
        {
            var customer = await customerRepository.GetByIdAsync(dto.Id);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with id {dto.Id} not found.");

            customer.ToCustomerEntity(dto);
            await customerRepository.UpdateAsync(customer);
        }
    }
}
