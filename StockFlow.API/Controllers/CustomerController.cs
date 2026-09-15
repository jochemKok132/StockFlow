using StockFlow.Application.DTOs.Customer;
using StockFlow.Application.DTOs.Customer.CustomerAuthentication;
using StockFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using StockFlow.Application.Interfaces.ManagementApp;
using System.Security.Claims;
using StockFlow.Application.DTOs.Pagination;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        [Authorize(Roles = "Manager,Admin,ShiftLeader")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCustomersAsync([FromQuery] CustomerPaginationDto pagination)
        {
            try
            {
                var customers = await customerService.GetAllCustomersAsync(pagination);
                return Ok(customers);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetCustomerByIdAsync(Guid id)
        {
            try
            {
                var customer = await customerService.GetCustomerByIdAsync(id);
                return Ok(customer);
            }
            catch (KeyNotFoundException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize]
        [HttpGet("GetByEmail/{email}")]
        public async Task<IActionResult> GetCustomerByEmailAsync(string email)
        {
            try
            {
                var customer = await customerService.GetCustomerByEmailAsync(email);
                return Ok(customer);
            }
            catch (KeyNotFoundException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateCustomerAsync(UpdateCustomerDto dto)
        {
            try
            {
                await customerService.UpdateCustomerAsync(dto);
                return NoContent();
            }
            catch (KeyNotFoundException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            try
            {
                var token = await customerService.LoginAsync(request);
                return Ok(token);
            }
            catch (KeyNotFoundException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (AuthenticationException exception)
            {
                return Unauthorized(exception.Message);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }
    }
}