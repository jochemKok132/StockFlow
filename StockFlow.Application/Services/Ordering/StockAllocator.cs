using StockFlow.Application.Interfaces.Ordering;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Repositories.Ordering;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Services.Ordering
{
    public class StockAllocator(ICustomerOrderRepository customerOrderRepository, IProductRepository productRepository) : IStockAllocator
    {
        public async Task<int> AllocateAsync(IEnumerable<Guid> productIds)
        {
            var ids = productIds.Distinct().ToList();
            if (ids.Count == 0) return 0;

            var waiting = await customerOrderRepository.GetWaitingForProductsAsync(ids);
            if (waiting.Count == 0) return 0;

            var products = await productRepository.GetByIdsAsync(ids);
            var reserved = await productRepository.GetReservedStockAsync(ids);
            var available = products.ToDictionary(p => p.Id, p => Math.Max(0, p.Stock - reserved.GetValueOrDefault(p.Id)));

            var changed = Allocate(waiting, available, DateTime.UtcNow);
            foreach (var order in changed)
                await customerOrderRepository.UpdateAsync(order);

            return changed.Count(o => o.Status == CustomerOrderStatus.ReadyForPickup);
        }

        public static List<CustomerOrder> Allocate(IEnumerable<CustomerOrder> waitingOrders, Dictionary<Guid, int> available, DateTime now)
        {
            var changed = new List<CustomerOrder>();
            foreach (var order in waitingOrders.Where(o => o.Status == CustomerOrderStatus.Waiting).OrderBy(o => o.CreatedAt).ThenBy(o => o.OrderNumber))
            {
                bool touched = false;
                foreach (var line in order.Lines.Where(l => l.ReservedAmount < l.Amount))
                {
                    int free = available.GetValueOrDefault(line.ProductId);
                    if (free <= 0) continue;

                    int take = Math.Min(free, line.Amount - line.ReservedAmount);
                    line.ReservedAmount += take;
                    available[line.ProductId] = free - take;
                    touched = true;
                }

                if (order.Lines.All(l => l.ReservedAmount >= l.Amount))
                {
                    order.Status = CustomerOrderStatus.ReadyForPickup;
                    order.ReadyAt = now;
                    touched = true;
                }

                if (touched) changed.Add(order);
            }
            return changed;
        }
    }
}
