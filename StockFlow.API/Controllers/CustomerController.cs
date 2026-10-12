using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Common;
using StockFlow.Application.DTOs.Customer;
using StockFlow.Application.DTOs.Customer.CustomerAuthentication;
using StockFlow.Application.DTOs.Pagination;
using StockFlow.Application.Interfaces;
using System.Security.Authentication;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        [Authorize(Roles = "Manager,Admin,ShiftLeader")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] CustomerPaginationDto pagination)
        {
            try
            {
                List<CustomerDto> customers = await customerService.GetAllCustomersAsync(pagination);
                return Ok(ApiResponse<List<CustomerDto>>.Ok(customers));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<CustomerDto>>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = AuthRoles.Employees)]
        [HttpGet("GetById/{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                CustomerDto customer = await customerService.GetCustomerByIdAsync(id);
                return Ok(ApiResponse<CustomerDto>.Ok(customer));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<CustomerDto>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = AuthRoles.Employees)]
        [HttpGet("GetByEmail/{email}")]
        public async Task<IActionResult> GetByEmailAsync(string email)
        {
            try
            {
                CustomerDto customer = await customerService.GetCustomerByEmailAsync(email);
                return Ok(ApiResponse<CustomerDto>.Ok(customer));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<CustomerDto>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync(UpdateCustomerDto request)
        {
            try
            {
                await customerService.UpdateCustomerAsync(request);
                return Ok(ApiResponse<object>.Ok(message: "Customer updated."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<object>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<object>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = AuthRoles.Customer)]
        [HttpGet("Me")]
        public async Task<IActionResult> GetMeAsync()
        {
            if (!User.TryGetUserId(out Guid customerId))
                return Unauthorized(ApiResponse<CustomerDto>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                CustomerDto customer = await customerService.GetCustomerByIdAsync(customerId);
                return Ok(ApiResponse<CustomerDto>.Ok(customer));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<CustomerDto>.Fail(exception.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                Token token = await customerService.RegisterCustomerAsync(request);
                return Ok(ApiResponse<Token>.Ok(token, "Account created."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<Token>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = AuthRoles.Customer)]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (!User.TryGetUserId(out Guid customerId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await customerService.ChangePasswordAsync(customerId, request);
                return Ok(ApiResponse<object>.Ok(message: "Password changed."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<object>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<object>.Fail(exception.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            try
            {
                Token token = await customerService.LoginAsync(request);
                return Ok(ApiResponse<Token>.Ok(token));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<Token>.Fail(exception.Message));
            }
            catch (Exception exception) when (exception is KeyNotFoundException or AuthenticationException)
            {
                return Unauthorized(ApiResponse<Token>.Fail("Email or password is incorrect."));
            }
        }
    }
}
