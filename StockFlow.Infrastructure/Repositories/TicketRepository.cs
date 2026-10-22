using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Interfaces.Repositories;
using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.Support;
using StockFlow.Domain.Enums;
using StockFlow.Infrastructure.Data;
using StockFlow.Infrastructure.Repositories.EfRepositories;

namespace StockFlow.Infrastructure.Repositories
{
    public class TicketRepository :
        EfRepository<Ticket>,
        IEfCreatableRepository<Ticket>,
        IEfUpdatableRepository<Ticket>,
        ITicketRepository
    {
        public const int FirstTicketNumber = 1001;
        private const int MaxTickets = 500;

        private readonly ApplicationDBContext _context;
        private readonly EfCreatableRepository<Ticket> _creatable;
        private readonly EfUpdatableRepository<Ticket> _updatable;

        public TicketRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
            _creatable = new EfCreatableRepository<Ticket>(context);
            _updatable = new EfUpdatableRepository<Ticket>(context);
        }

        public Task AddAsync(Ticket entity) => _creatable.AddAsync(entity);
        public Task UpdateAsync(Ticket entity) => _updatable.UpdateAsync(entity);

        public async Task<int> GetNextTicketNumberAsync()
        {
            int? highest = await _context.Tickets.MaxAsync(t => (int?)t.TicketNumber);
            return highest is int number ? number + 1 : FirstTicketNumber;
        }

        public Task<Ticket?> GetByNumberAsync(int ticketNumber) =>
            WithEmployees().FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);

        public async Task<IEnumerable<Ticket>> GetForEmployeeAsync(Guid employeeId) =>
            await WithEmployees()
                .AsNoTracking()
                .Where(t => t.CreatedById == employeeId)
                .OrderByDescending(t => t.TicketNumber)
                .Take(MaxTickets)
                .ToListAsync();

        public async Task<IEnumerable<Ticket>> GetByTypesAsync(IReadOnlyCollection<TicketType> types, TicketStatus? status) =>
            await WithEmployees()
                .AsNoTracking()
                .Where(t => types.Contains(t.Type) && (status == null || t.Status == status))
                .OrderBy(t => t.Status == TicketStatus.Done || t.Status == TicketStatus.Rejected)
                .ThenByDescending(t => t.TicketNumber)
                .Take(MaxTickets)
                .ToListAsync();

        private IQueryable<Ticket> WithEmployees() =>
            _context.Tickets.Include(t => t.CreatedBy).Include(t => t.HandledBy);
    }
}
