using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Ordering;

namespace StockFlow.Application.Services.Ordering
{
    public class StockAutomationService(IProductControlService productControlService, IStockCountService stockCountService, IPurchaseOrderService purchaseOrderService) : IStockAutomationService
    {
        public async Task<StockAutomationResult> RunAsync(DateTime today)
        {
            int prices = await productControlService.ApplyDuePricesAsync(today);
            int counts = await stockCountService.CreateExpiryCountsAsync(today);
            int lines = await purchaseOrderService.CreateAutomaticOrdersAsync(today);
            int sent = await purchaseOrderService.SendDueOrdersAsync(today);
            return new StockAutomationResult(prices, counts, lines, sent);
        }
    }
}
