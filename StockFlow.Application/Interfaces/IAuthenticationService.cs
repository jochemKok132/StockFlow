using StockFlow.Shared.DTOs.Customer.CustomerAuthentication;
using StockFlow.Shared.DTOs.Employee.EmployeeAuthentication;
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
        Shared.DTOs.Customer.CustomerAuthentication.Token GenerateToken(Customer user);
        Shared.DTOs.Employee.EmployeeAuthentication.Token GenerateToken(Employee user);
    }
}
