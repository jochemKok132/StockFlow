using StockFlow.ManagementApp.Interfaces;
using StockFlow.ManagementApp.Interfaces.Stock;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Supplier;

namespace StockFlow.ManagementApp.Services.Stock
{
    public class SupplierService(IHttpService httpService) : ISupplierService
    {
        public async Task<(List<SupplierDto>, string error)> GetPaginatedSuppliersAsync(SupplierPaginationDto pagination)
        {
            var url = "Supplier/GetAll" +
                $"?pageNumber={pagination.PageNumber}" +
                $"&pageSize={pagination.PageSize}" +
                $"&orderBy={pagination.OrderBy}" +
                $"&orderType={pagination.OrderType}" +
                $"&supplierName={Uri.EscapeDataString(pagination.SupplierName ?? "")}";
            var response = await httpService.GetAsync<List<SupplierDto>>(url);
            return response.Succeeded ? (response.Value ?? [], "") : ([], $"Error: {response.Message}");
        }

        public async Task<ApiResult<SupplierDto>> CreateSupplierAsync(CreateSupplierDto supplier) =>
            ApiResult<SupplierDto>.From(await httpService.PostAsync<SupplierDto, CreateSupplierDto>("Supplier/Create", supplier), "The supplier could not be created.");

        public async Task<ApiResult<SupplierDto>> UpdateSupplierAsync(UpdateSupplierDto supplier) =>
            ApiResult<SupplierDto>.From(await httpService.PutAsync<SupplierDto, UpdateSupplierDto>("Supplier/Update", supplier), "The supplier could not be saved.");

        public async Task<ApiResult<object>> SoftDeleteSupplierAsync(Guid id) =>
            ApiResult<object>.From(await httpService.DeleteAsync<object>($"Supplier/SoftDelete/{id}"), "The supplier could not be deleted.");
    }
}
