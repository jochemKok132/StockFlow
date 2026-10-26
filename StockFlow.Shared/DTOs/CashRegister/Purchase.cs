using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Employee;
using StockFlow.Shared.DTOs.Stock.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.CashRegister
{
    public class Purchase
    {
        public Guid? CustomerId { get; set; }
        public double TotalPrice { get; set; }
        public double TotalOff { get; set; }
        public int PointsUsed { get; set; }
        public int? CustomerOrderNumber { get; set; }
        public IEnumerable<RegisterItemDetailDto> ProductsSold { get; set; } = [];
    }
}
