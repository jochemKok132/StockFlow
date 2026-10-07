using StockFlow.Application.DTOs.AuditLogs.Customer;
using StockFlow.Application.DTOs.CashRegister;
using StockFlow.Application.DTOs.Stock.Product;
using StockFlow.CashRegister.Interfaces;
using StockFlow.Domain.Entities.Stock;

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
            // The controller route is [controller]/{input}, so the CashRegister/ prefix is required.
            var response = await httpService.GetAsync<ProductDto>($"CashRegister/{input}");

            if (!response.Succeeded)
                return (new ProductDto(), $"Error: {response.Message}");

            // The API answers 204 NoContent when nothing matches.
            if (response.Value is null)
                return (new ProductDto(), "Error: Product niet gevonden.");

            return (response.Value, "");
        }
    }
}