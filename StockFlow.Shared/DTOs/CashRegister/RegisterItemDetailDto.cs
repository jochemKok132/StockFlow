using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Stock.Sales;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Shared.DTOs.CashRegister
{
    public class RegisterItemDetailDto
    {
        public Guid Id { get; set; }
        public ProductDto Product { get; set; }
        public int Amount { get; set; }
        public int ReturnedAmount { get; set; }
        public decimal PriceAtTimeOfSale { get; set; }
        public SalesDto? Sale { get; set; }
    }
}
