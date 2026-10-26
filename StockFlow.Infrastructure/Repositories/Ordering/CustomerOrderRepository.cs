using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Application.Interfaces.Repositories.Ordering;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Enums;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories.EfRepositories;
using StockFlow.Shared.Common;

namespace StockFlow.Infrastructure.Repositories.Ordering
{
    public class CustomerOrderRepository :
        EfRepository<CustomerOrder>,
        IEfCreatableRepository<CustomerOrder>,
        IEfUpdatableRepository<CustomerOrder>,
        ICustomerOrderRepository
    {
        private const int MaxOrders = 300;
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<CustomerOrder> _creatable;
        private readonly EfUpdatableRepository<CustomerOrder> _updatable;

        public CustomerOrderRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<CustomerOrder>(context);
            _updatable = new EfUpdatableRepository<CustomerOrder>(context);
        }

        public Task AddAsync(CustomerOrder entity) => _creatable.AddAsync(entity);
        public Task UpdateAsync(CustomerOrder entity) => _updatable.UpdateAsync(entity);

        public async Task<int> GetNextOrderNumberAsync()
        {
            int? highest = await _context.CustomerOrders.MaxAsync(o => (int?)o.OrderNumber);
            int next = highest is int number ? number + 1 : IdentifierFormats.FirstCustomerOrderNumber;
            if (next > IdentifierFormats.LastCustomerOrderNumber)
                throw new InvalidOperationException("All customer order numbers are used.");
            return next;
        }

        public Task<CustomerOrder?> GetByNumberAsync(int orderNumber) =>
            WithDetails().FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        public Task<List<CustomerOrder>> GetListAsync(CustomerOrderStatus? status) =>
            WithDetails()
                .AsNoTracking()
                .Where(o => status == null || o.Status == status)
                .OrderBy(o => o.Status == CustomerOrderStatus.PickedUp || o.Status == CustomerOrderStatus.Cancelled)
                .ThenByDescending(o => o.OrderNumber)
                .Take(MaxOrders)
                .ToListAsync();

        public Task<List<CustomerOrder>> GetWaitingForProductsAsync(IEnumerable<Guid> productIds)
        {
            var ids = productIds.Distinct().ToList();
            return _context.CustomerOrders
                .Include(o => o.Lines)
                .Where(o => o.Status == CustomerOrderStatus.Waiting && o.Lines.Any(l => ids.Contains(l.ProductId)))
                .OrderBy(o => o.CreatedAt)
                .ThenBy(o => o.OrderNumber)
                .ToListAsync();
        }

        private IQueryable<CustomerOrder> WithDetails() =>
            _context.CustomerOrders
                .Include(o => o.Customer)
                .Include(o => o.CreatedBy)
                .Include(o => o.Lines).ThenInclude(l => l.Product)
                .AsSplitQuery();
    }
}
