using StockFlow.ManagementApp.Interfaces;
using StockFlow.Shared.DTOs.Releases;

namespace StockFlow.ManagementApp.Services
{
    public class ReleaseService(IHttpService httpService) : IReleaseService
    {
        public async Task<(List<ReleaseDto>, string error)> GetReleasesAsync()
        {
            var response = await httpService.GetAsync<List<ReleaseDto>>("Releases");
            return response.Succeeded
                ? (response.Value ?? [], "")
                : ([], response.Message ?? "The releases could not be loaded.");
        }

        public async Task<string?> GetApiVersionAsync()
        {
            var response = await httpService.GetAsync<string>("Releases/Version");
            return response.Succeeded ? response.Value : null;
        }
    }
}
