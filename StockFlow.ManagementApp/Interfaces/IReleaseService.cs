using StockFlow.Shared.DTOs.Releases;

namespace StockFlow.ManagementApp.Interfaces
{
    public interface IReleaseService
    {
        Task<(List<ReleaseDto>, string error)> GetReleasesAsync();
        Task<string?> GetApiVersionAsync();
    }
}
