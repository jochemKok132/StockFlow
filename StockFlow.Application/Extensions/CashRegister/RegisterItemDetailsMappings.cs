using StockFlow.Application.DTOs.CashRegister;
using StockFlow.Application.Mappings.Stock;
using StockFlow.Domain.Entities.CashRegister;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Extensions.CashRegister
{
    public static class RegisterItemDetailsMappings
    {
        public static RegisterItemDetailDto ToRegisterItemDetailDto(this RegisterItemDetail dto)
        {
            return new RegisterItemDetailDto()
            {
                Id = dto.Id,
                Amount = dto.Amount,
                PriceAtTimeOfSale = dto.PriceAtTimeOfSale,
                Product = dto.Product.ToProductDto(),
                Sale = dto.Sale?.ToSalesDto(),
            };
        }
        public static RegisterItemDetail ToRegisterItemDetailEntity(this RegisterItemDetailDto dto)
        {
            return new RegisterItemDetail()
            {
                Id = Guid.NewGuid(),
                Amount = dto.Amount,
                PriceAtTimeOfSale = dto.PriceAtTimeOfSale,
                ProductId = dto.Product.Id,
                SalesId = dto.Sale is null || dto.Sale.Id == Guid.Empty ? null : dto.Sale.Id,
            };
        }
    }
}
