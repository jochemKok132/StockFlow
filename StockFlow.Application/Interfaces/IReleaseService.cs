using StockFlow.Shared.DTOs.Releases;

namespace StockFlow.Application.Interfaces
{
    public interface IReleaseService
    {
        Task<List<ReleaseDto>> GetReleasesAsync();
        Task<ReleaseDto?> GetLatestReleaseAsync();
    }
}
