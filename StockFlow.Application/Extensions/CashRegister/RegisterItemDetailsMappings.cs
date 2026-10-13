using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.DTOs.Stock.Product;
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
                ReturnedAmount = dto.ReturnedAmount,
                PriceAtTimeOfSale = dto.PriceAtTimeOfSale,
                Product = dto.Product?.ToProductDto() ?? new ProductDto(),
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
