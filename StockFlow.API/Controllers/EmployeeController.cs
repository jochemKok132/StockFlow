using StockFlow.Application.DTOs.Employee;
using StockFlow.Application.DTOs.Employee.EmployeeAuthentication;
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
    public class EmployeeController(IEmployeeService employeeService) : ControllerBase
    {
        [Authorize(Roles = "Manager,Admin,ShiftLeader")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllEmployeesAsync([FromQuery] EmployeePaginationDto pagination)
        {
            try
            {
                var employees = await employeeService.GetAllEmployeesAsync(pagination);
                return Ok(employees);
            }
            catch (ArgumentException)
            {
                return BadRequest("There has been an unforseen error.");
            }
        }

        [Authorize]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetEmployeeByIdAsync(Guid id)
        {
            try
            {
                var employee = await employeeService.GetEmployeeByIdAsync(id);
                return Ok(employee);
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
        [HttpGet("GetByEmployeeId/{employeeId}")]
        public async Task<IActionResult> GetEmployeeByEmployeeIdAsync(string employeeId)
        {
            try
            {
                var employee = await employeeService.GetEmployeeByEmployeeIdAsync(employeeId);
                return Ok(employee);
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
        [HttpPost("Create")]
        public async Task<IActionResult> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await employeeService.CreateEmployeeAsync(dto, Guid.Parse(userId));
                return NoContent();
            }
            catch (ArgumentNullException exception)
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
        public async Task<IActionResult> UpdateEmployeeAsync(UpdateEmployeeDto dto)
        {
            var userId = Request.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await employeeService.UpdateEmployeeAsync(dto, Guid.Parse(userId));
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
                var token = await employeeService.LoginAsync(request);
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