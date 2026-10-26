using StockFlow.Shared.DTOs.Enums;
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
                LogTypeDto.Login => LogType.Login,
                LogTypeDto.Sold => LogType.Sold,
                LogTypeDto.Returned => LogType.Returned,
                LogTypeDto.WrittenOff => LogType.WrittenOff,
                LogTypeDto.Restored => LogType.Restored,
                LogTypeDto.Counted => LogType.Counted,
                LogTypeDto.Received => LogType.Received,
                LogTypeDto.Recalled => LogType.Recalled,
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
                LogType.Login => LogTypeDto.Login,
                LogType.Sold => LogTypeDto.Sold,
                LogType.Returned => LogTypeDto.Returned,
                LogType.WrittenOff => LogTypeDto.WrittenOff,
                LogType.Restored => LogTypeDto.Restored,
                LogType.Counted => LogTypeDto.Counted,
                LogType.Received => LogTypeDto.Received,
                LogType.Recalled => LogTypeDto.Recalled,
                _ => throw new ArgumentOutOfRangeException(nameof(logType))
            };
        }
    }
}
