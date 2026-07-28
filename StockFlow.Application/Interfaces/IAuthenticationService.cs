using Application.DTOs.Authentication;
using Domain.Entities;
using StockFlow.Application.DTOs.Customer.CustomerAuthentication;
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
        Token GenerateToken(Customer user);
        Token GenerateToken(Employee user);
    }
}
