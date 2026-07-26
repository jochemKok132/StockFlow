using StockFlow.Application.DTOs.Enums;
using StockFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockFlow.Application.Mappings.Enums
{
    public static class LogTypeMappings
    {
        public static LogType ToDomain(this LogTypeDto logType)
        {
            return logType switch
            {
                LogTypeDto.Create => LogType.Create,
                LogTypeDto.Update => LogType.Update,
                LogTypeDto.Delete => LogType.Delete,
                _ => throw new ArgumentOutOfRangeException(nameof(logType))
            };
        }

        public static LogTypeDto ToDto(this LogType logType)
        {
            return logType switch
            {
                LogType.Create => LogTypeDto.Create,
                LogType.Update => LogTypeDto.Update,
                LogType.Delete => LogTypeDto.Delete,
                _ => throw new ArgumentOutOfRangeException(nameof(logType))
            };
        }
    }
}
