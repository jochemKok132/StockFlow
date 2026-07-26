using StockFlow.Application.DTOs.Enums;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.Enums
{
    public static class StockLogTypeMappings
    {
        public static StockLogType ToDomain(this StockLogTypeDto logType)
        {
            return logType switch
            {
                StockLogTypeDto.Product => StockLogType.Product,
                StockLogTypeDto.ProductBrand => StockLogType.ProductBrand,
                StockLogTypeDto.Sales => StockLogType.Sales,
                StockLogTypeDto.Shelf => StockLogType.Shelf,
                _ => throw new ArgumentOutOfRangeException(nameof(logType))
            };
        }

        public static StockLogTypeDto ToDto(this StockLogType logType)
        {
            return logType switch
            {
                StockLogType.Product => StockLogTypeDto.Product,
                StockLogType.ProductBrand => StockLogTypeDto.ProductBrand,
                StockLogType.Sales => StockLogTypeDto.Sales,
                StockLogType.Shelf => StockLogTypeDto.Shelf,
                _ => throw new ArgumentOutOfRangeException(nameof(logType))
            };
        }
    }
}
