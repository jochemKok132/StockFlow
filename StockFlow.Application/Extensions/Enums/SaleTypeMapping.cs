using StockFlow.Domain.Enums;
using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Application.Mappings.Enums
{
    public static class SaleTypeMapping
    {
        public static SaleType ToDomain(this SaleTypeDto type)
        {
            return type switch
            {
                SaleTypeDto.Percentage => SaleType.Percentage,
                SaleTypeDto.MultiBuy => SaleType.MultiBuy,
                SaleTypeDto.Moving => SaleType.Moving,
                SaleTypeDto.SecondItemHalfPrice => SaleType.SecondItemHalfPrice,
                SaleTypeDto.Bundle => SaleType.Bundle,
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        public static SaleTypeDto ToDto(this SaleType type)
        {
            return type switch
            {
                SaleType.Percentage => SaleTypeDto.Percentage,
                SaleType.MultiBuy => SaleTypeDto.MultiBuy,
                SaleType.Moving => SaleTypeDto.Moving,
                SaleType.SecondItemHalfPrice => SaleTypeDto.SecondItemHalfPrice,
                SaleType.Bundle => SaleTypeDto.Bundle,
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }
    }
}
