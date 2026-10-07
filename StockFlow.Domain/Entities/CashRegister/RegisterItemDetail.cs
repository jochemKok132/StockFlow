using StockFlow.Domain.Entities.Stock;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Domain.Entities.CashRegister
{
    public class RegisterItemDetail
    {
        public Guid Id { get; set; }
        public Guid CashregisterLogId { get; set; }
        public int Amount { get; set; }
        public decimal PriceAtTimeOfSale { get; set; }

        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid? SalesId { get; set; }
        public Sales? Sale { get; set; }
    }
}
