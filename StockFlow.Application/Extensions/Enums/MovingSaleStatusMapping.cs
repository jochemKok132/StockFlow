using StockFlow.Domain.Enums;
using StockFlow.Shared.DTOs.Enums;

namespace StockFlow.Application.Mappings.Enums
{
    public static class MovingSaleStatusMapping
    {
        public static MovingSaleStatusDto ToDto(this MovingSaleStatus status)
        {
            return status switch
            {
                MovingSaleStatus.None => MovingSaleStatusDto.None,
                MovingSaleStatus.Pending => MovingSaleStatusDto.Pending,
                MovingSaleStatus.Approved => MovingSaleStatusDto.Approved,
                MovingSaleStatus.Rejected => MovingSaleStatusDto.Rejected,
                _ => throw new ArgumentOutOfRangeException(nameof(status))
            };
        }
    }
}
