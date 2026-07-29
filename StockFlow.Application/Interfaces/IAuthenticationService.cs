using StockFlow.Application.DTOs.Customer.CustomerAuthentication;
using StockFlow.Application.DTOs.Employee.EmployeeAuthentication;
using StockFlow.Domain.Entities.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application.Interfaces
{
    public interface IAuthenticationService
    {
        DTOs.Customer.CustomerAuthentication.Token GenerateToken(Customer user);
        DTOs.Employee.EmployeeAuthentication.Token GenerateToken(Employee user);
    }
}
