using StockFlow.ManagementApp.Components;

namespace StockFlow.ManagementApp.Interfaces
{
    public interface ILookupService
    {
        Task<(List<SelectOption>, string error)> GetBrandOptionsAsync();
        Task<(List<SelectOption>, string error)> GetShelfOptionsAsync();
        Task<(List<SelectOption>, string error)> GetSalesTagOptionsAsync();
        Task<(List<SelectOption>, string error)> GetSupplierOptionsAsync();
        Task<(List<SelectOption>, string error)> GetActiveSaleOptionsAsync();
    }
}
