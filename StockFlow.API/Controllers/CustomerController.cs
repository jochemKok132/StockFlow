using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Customer.CustomerAuthentication;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Application.Interfaces;
using System.Security.Authentication;
using System.Security.Claims;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        [Authorize(Roles = Permissions.ViewCustomers)]
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

        [Authorize(Roles = Permissions.Employees)]
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

        [Authorize(Roles = Permissions.Employees)]
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

        [Authorize(Roles = Permissions.ManageCustomers)]
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

        [Authorize(Roles = AuthRoles.Customer)]
        [HttpPost("MovingSale")]
        public async Task<IActionResult> RequestMovingSaleAsync(MovingSaleRequest request)
        {
            if (!User.TryGetUserId(out Guid customerId))
                return Unauthorized(ApiResponse<CustomerDto>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                CustomerDto customer = await customerService.RequestMovingSaleAsync(customerId, request);
                return Ok(ApiResponse<CustomerDto>.Ok(customer, "Your moving sale request was sent. A shift leader will check it."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<CustomerDto>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<CustomerDto>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.ViewMovingSales)]
        [HttpGet("MovingSaleRequests")]
        public async Task<IActionResult> GetMovingSaleRequestsAsync()
        {
            List<CustomerDto> customers = await customerService.GetMovingSaleRequestsAsync();
            return Ok(ApiResponse<List<CustomerDto>>.Ok(customers));
        }

        [Authorize(Roles = Permissions.ApproveMovingSales)]
        [HttpPost("MovingSale/{id:guid}/Approve")]
        public Task<IActionResult> ApproveMovingSaleAsync(Guid id) =>
            ReviewMovingSaleAsync(() => customerService.ApproveMovingSaleAsync(id, ReviewerId()), "Moving sale approved.");

        [Authorize(Roles = Permissions.ApproveMovingSales)]
        [HttpPost("MovingSale/{id:guid}/Reject")]
        public Task<IActionResult> RejectMovingSaleAsync(Guid id) =>
            ReviewMovingSaleAsync(() => customerService.RejectMovingSaleAsync(id, ReviewerId()), "Moving sale rejected.");

        private string ReviewerId() => User.FindFirstValue(ClaimTypes.Thumbprint) ?? "";

        private async Task<IActionResult> ReviewMovingSaleAsync(Func<Task<CustomerDto>> review, string message)
        {
            try
            {
                CustomerDto customer = await review();
                return Ok(ApiResponse<CustomerDto>.Ok(customer, message));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<CustomerDto>.Fail(exception.Message));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<CustomerDto>.Fail(exception.Message));
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
