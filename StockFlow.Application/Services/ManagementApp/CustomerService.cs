using StockFlow.Application.DTOs.Customer;
using StockFlow.Application.Common;
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

using StockFlow.Application.Interfaces.ManagementApp.AuditLogs;
using StockFlow.Application.Mappings.AuditLogs;
using StockFlow.Application.Validations;
using StockFlow.Application.Validations.People;
using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.DTOs.Enums;

namespace StockFlow.Application.Services.ManagementApp
{
    public class CustomerService(ICustomerRepository customerRepository, IAuthenticationService authenticationService, IPassword password, ICustomerLogsService customerLogsService) : ICustomerService
    {
        public async Task<List<CustomerDto>> GetAllCustomersAsync(CustomerPaginationDto pagination)
        {
            pagination.Validate();

            var customers = await customerRepository.GetAllCustomersAsync(pagination);
            return customers.Select(e => e.ToCustomerDto()).ToList();
        }

        public async Task<CustomerDto> GetCustomerByEmailAsync(string email)
        {
            var customer = await customerRepository.GetCustomerByEmailAsync(email);
            if (customer == null) throw new KeyNotFoundException($"Customer with email {email} not found.");
            return customer.ToCustomerDto();
        }

        public async Task<CustomerDto> GetCustomerByCustomerIdAsync(string customerId)
        {
            var customer = await customerRepository.GetCustomerByCustomerIdAsync(customerId);
            if (customer == null) throw new KeyNotFoundException($"Customer with customer id {customerId} not found.");
            return customer.ToCustomerDto();
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(Guid id)
        {
            var customer = await customerRepository.GetByIdAsync(id);
            if (customer == null || customer.SoftDeleted) throw new KeyNotFoundException($"Customer with id {id} not found.");
            return customer.ToCustomerDto();
        }

        public async Task<Token> LoginAsync(LoginRequest request)
        {
            request.Validate();

            Customer? customer = await customerRepository.GetCustomerByEmailAsync(request.Email);

            if (customer == null)
                throw new KeyNotFoundException("The Customer with this id cannot be found.");

            if (string.IsNullOrEmpty(customer.HashedPassword) || !password.Validate(customer.HashedPassword, request.Password))
                throw new AuthenticationException("The password is incorrect.");

            await customerLogsService.CreateCustomerLogsAsync(new CreateCustomerLogsDto
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                LogType = LogTypeDto.Login,
            });

            return authenticationService.GenerateToken(customer);
        }

        public async Task<Token> RegisterCustomerAsync(RegisterRequest register)
        {
            register.Validate();

            var email = register.Email.Trim().ToLower();
            if (await customerRepository.GetCustomerByEmailAsync(email) is not null)
                throw new ArgumentException("An account with this email already exists.");

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                CustomerId = await customerRepository.GetNewCustomerIdAsync(),
                FullName = register.Name.Trim(),
                Email = email,
                PostalCode = CustomerValidation.NormalizePostalCode(register.PostalCode),
                HouseNumber = register.HouseNumber.Trim().ToUpperInvariant(),
                HashedPassword = password.Hash(register.Password),
                SavedPoints = LoyaltyPoints.WelcomePoints,
            };

            await customerRepository.AddAsync(customer);

            await customerLogsService.CreateCustomerLogsAsync(new CreateCustomerLogsDto
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                LogType = LogTypeDto.Create,
                Messages = [$"SavedPoints: {LoyaltyPoints.WelcomePoints} (welcome points)"],
            });

            return authenticationService.GenerateToken(customer);
        }

        public async Task ChangePasswordAsync(Guid customerId, ChangePasswordRequest request)
        {
            request.Validate();

            var customer = await customerRepository.GetByIdAsync(customerId);
            if (customer == null || customer.SoftDeleted)
                throw new KeyNotFoundException("Customer not found.");

            if (string.IsNullOrEmpty(customer.HashedPassword) || !password.Validate(customer.HashedPassword, request.CurrentPassword))
                throw new ArgumentException("The current password is incorrect.");

            customer.HashedPassword = password.Hash(request.NewPassword);
            await customerRepository.UpdateAsync(customer);

            await customerLogsService.CreateCustomerLogsAsync(new CreateCustomerLogsDto
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                LogType = LogTypeDto.Update,
                Messages = ["Password: Changed."],
            });
        }

        public async Task UpdateCustomerAsync(UpdateCustomerDto dto)
        {
            dto.Validate();

            var customer = await customerRepository.GetByIdAsync(dto.Id);
            if (customer == null)
                throw new KeyNotFoundException($"Customer with id {dto.Id} not found.");

            var log = dto.ToUpdateCustomerLogsDto(customer);

            customer.ToCustomerEntity(dto);
            await customerRepository.UpdateAsync(customer);
            await customerLogsService.CreateCustomerLogsAsync(log);
        }
    }
}
