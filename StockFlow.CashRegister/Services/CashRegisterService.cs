using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.AuditLogs.Customer;
using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Releases;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.CashRegister.Interfaces;

namespace StockFlow.CashRegister.Services
{
    public class CashRegisterService(IHttpService httpService) : ICashRegisterService
    {
        public async Task<string> FinalizePurchaseAsync(Purchase purchase)
        {
            var response = await httpService.PostAsync<string, Purchase>("CashRegister/Purchase", purchase);
            if (response.Succeeded)
                return response.Value ?? string.Empty;
            else
                return $"Error: {response.Message}";
        }

        public async Task<(ProductDto, string error)> GetProductAsync(string input)
        {
            var response = await httpService.GetAsync<ProductDto>($"CashRegister/{input}");

            if (!response.Succeeded)
                return (new ProductDto(), $"Error: {response.Message}");

            if (response.Value is null)
                return (new ProductDto(), "Error: Product niet gevonden.");

            return (response.Value, "");
        }

        public async Task<(CustomerDto?, string error)> GetCustomerAsync(string customerId)
        {
            var response = await httpService.GetAsync<CustomerDto>($"CashRegister/Customer/{customerId}");

            if (!response.Succeeded)
                return (null, $"Error: {response.Message}");

            if (response.Value is null)
                return (null, "Error: Klant niet gevonden.");

            return (response.Value, "");
        }

        public async Task<(CashRegisterLogsDto?, string error)> GetReceiptAsync(string receiptId)
        {
            var response = await httpService.GetAsync<CashRegisterLogsDto>($"CashRegister/Receipt/{receiptId}");

            if (!response.Succeeded)
                return (null, $"Error: {response.Message}");

            if (response.Value is null)
                return (null, "Error: Bon niet gevonden.");

            return (response.Value, "");
        }

        public async Task<(ReturnResult?, string error)> ProcessReturnAsync(ReturnRequest request)
        {
            var response = await httpService.PostAsync<ReturnResult, ReturnRequest>("CashRegister/Return", request);

            if (!response.Succeeded)
                return (null, $"Error: {response.Message}");

            if (response.Value is null)
                return (null, "Error: De retour kon niet worden verwerkt.");

            return (response.Value, "");
        }

        public async Task<ReleaseDto?> GetLatestReleaseAsync()
        {
            var response = await httpService.GetAsync<ReleaseDto>("Releases/Latest");
            return response.Succeeded ? response.Value : null;
        }
    }
}