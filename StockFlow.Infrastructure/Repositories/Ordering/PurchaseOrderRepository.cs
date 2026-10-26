using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Application.Interfaces.Repositories.Ordering;
using StockFlow.Domain.Entities.Ordering;
using StockFlow.Domain.Enums;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories.EfRepositories;

namespace StockFlow.Infrastructure.Repositories.Ordering
{
    public class PurchaseOrderRepository :
        EfRepository<PurchaseOrder>,
        IEfCreatableRepository<PurchaseOrder>,
        IEfUpdatableRepository<PurchaseOrder>,
        IPurchaseOrderRepository
    {
        public const int FirstOrderNumber = 1001;
        private const int MaxOrders = 300;
        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<PurchaseOrder> _creatable;
        private readonly EfUpdatableRepository<PurchaseOrder> _updatable;

        public PurchaseOrderRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<PurchaseOrder>(context);
            _updatable = new EfUpdatableRepository<PurchaseOrder>(context);
        }

        public Task AddAsync(PurchaseOrder entity) => _creatable.AddAsync(entity);
        public Task UpdateAsync(PurchaseOrder entity) => _updatable.UpdateAsync(entity);

        public async Task<int> GetNextOrderNumberAsync()
        {
            int? highest = await _context.PurchaseOrders.MaxAsync(o => (int?)o.OrderNumber);
            return highest is int number ? number + 1 : FirstOrderNumber;
        }

        public Task<PurchaseOrder?> GetByNumberAsync(int orderNumber) =>
            WithDetails().FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        public Task<PurchaseOrder?> GetOpenForSupplierAsync(Guid supplierId) =>
            WithDetails()
                .Where(o => o.SupplierId == supplierId && o.Status == PurchaseOrderStatus.Open)
                .OrderBy(o => o.OrderDate)
                .FirstOrDefaultAsync();

        public Task<List<PurchaseOrder>> GetOpenOrdersForSupplierAsync(Guid supplierId) =>
            _context.PurchaseOrders
                .Where(o => o.SupplierId == supplierId && o.Status == PurchaseOrderStatus.Open)
                .ToListAsync();

        public Task<List<PurchaseOrder>> GetListAsync(PurchaseOrderStatus? status, Guid? supplierId) =>
            WithDetails()
                .AsNoTracking()
                .Where(o => (status == null || o.Status == status) && (supplierId == null || o.SupplierId == supplierId))
                .OrderBy(o => o.Status == PurchaseOrderStatus.Received || o.Status == PurchaseOrderStatus.Cancelled)
                .ThenByDescending(o => o.OrderNumber)
                .Take(MaxOrders)
                .ToListAsync();

        public Task<List<PurchaseOrder>> GetOpenOrdersDueAsync(DateTime today) =>
            _context.PurchaseOrders
                .Include(o => o.Lines)
                .Include(o => o.Supplier)
                .Where(o => o.Status == PurchaseOrderStatus.Open && o.OrderDate <= today.Date)
                .ToListAsync();

        public Task<List<PurchaseOrderLine>> GetOpenLinesForCustomerOrderAsync(Guid customerOrderId) =>
            _context.PurchaseOrderLines
                .Include(l => l.PurchaseOrder)
                .Where(l => l.CustomerOrderId == customerOrderId && l.PurchaseOrder!.Status == PurchaseOrderStatus.Open)
                .ToListAsync();

        public Task<List<PurchaseOrderLine>> GetOpenLinesForProductAsync(Guid productId) =>
            _context.PurchaseOrderLines
                .Include(l => l.PurchaseOrder)
                .Where(l => l.ProductId == productId && l.PurchaseOrder!.Status == PurchaseOrderStatus.Open)
                .ToListAsync();

        public async Task<Dictionary<Guid, int>> GetCustomerOrderNumbersAsync(IEnumerable<Guid> customerOrderIds)
        {
            var ids = customerOrderIds.Distinct().ToList();
            return await _context.CustomerOrders
                .AsNoTracking()
                .Where(o => ids.Contains(o.Id))
                .ToDictionaryAsync(o => o.Id, o => o.OrderNumber);
        }

        public void RemoveLine(PurchaseOrderLine line)
        {
            line.PurchaseOrder?.Lines.Remove(line);
            _context.PurchaseOrderLines.Remove(line);
        }

        private IQueryable<PurchaseOrder> WithDetails() =>
            _context.PurchaseOrders
                .Include(o => o.Supplier)
                .Include(o => o.ReceivedBy)
                .Include(o => o.Lines).ThenInclude(l => l.Product)
                .AsSplitQuery();
    }
}
