using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.Application.DTOs.Stock.Sales;
using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.DTOs.CashRegister
{
    public class RegisterItemDetailDto
    {
        public Guid Id { get; set; }
        public ProductDto Product { get; set; }
        public int Amount { get; set; }
        public decimal PriceAtTimeOfSale { get; set; }
        public SalesDto? Sale { get; set; }
    }
}
