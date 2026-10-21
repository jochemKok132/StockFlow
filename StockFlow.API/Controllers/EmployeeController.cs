using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Employee;
using StockFlow.Shared.DTOs.Employee.EmployeeAuthentication;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Application.Interfaces.ManagementApp;
using System.Security.Authentication;

namespace StockFlow.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController(IEmployeeService employeeService) : ControllerBase
    {
        [Authorize(Roles = Permissions.ViewEmployees)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] EmployeePaginationDto pagination)
        {
            try
            {
                List<EmployeeDto> employees = await employeeService.GetAllEmployeesAsync(pagination, User.EmployeeRole());
                return Ok(ApiResponse<List<EmployeeDto>>.Ok(employees));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<List<EmployeeDto>>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.Employees)]
        [HttpGet("GetById/{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                EmployeeDto employee = await employeeService.GetEmployeeByIdAsync(id, User.EmployeeRole());
                return Ok(ApiResponse<EmployeeDto>.Ok(employee));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<EmployeeDto>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.Employees)]
        [HttpGet("GetByEmployeeId/{employeeId}")]
        public async Task<IActionResult> GetByEmployeeIdAsync(string employeeId)
        {
            try
            {
                EmployeeDto employee = await employeeService.GetEmployeeByEmployeeIdAsync(employeeId, User.EmployeeRole());
                return Ok(ApiResponse<EmployeeDto>.Ok(employee));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(ApiResponse<EmployeeDto>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.ManageEmployees)]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateAsync(CreateEmployeeDto request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await employeeService.CreateEmployeeAsync(request, userId, User.EmployeeRole());
                return Ok(ApiResponse<object>.Ok(message: "Employee created."));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<object>.Fail(exception.Message));
            }
        }

        [Authorize(Roles = Permissions.ManageEmployees)]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateAsync(UpdateEmployeeDto request)
        {
            if (!User.TryGetUserId(out Guid userId))
                return Unauthorized(ApiResponse<object>.Fail(ApiMessages.NotLoggedIn));

            try
            {
                await employeeService.UpdateEmployeeAsync(request, userId, User.EmployeeRole());
                return Ok(ApiResponse<object>.Ok(message: "Employee updated."));
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
                Token token = await employeeService.LoginAsync(request);
                return Ok(ApiResponse<Token>.Ok(token));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(ApiResponse<Token>.Fail(exception.Message));
            }
            catch (Exception exception) when (exception is KeyNotFoundException or AuthenticationException)
            {
                return Unauthorized(ApiResponse<Token>.Fail("Employee Id or password is incorrect."));
            }
        }
    }
}
