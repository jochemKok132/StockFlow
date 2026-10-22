using StockFlow.Application.Interfaces.Repositories.EfRepositories;
using StockFlow.Domain.Entities.Support;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Interfaces.Repositories
{
    public interface ITicketRepository :
        IEfRepository<Ticket>,
        IEfCreatableRepository<Ticket>,
        IEfUpdatableRepository<Ticket>
    {
        Task<int> GetNextTicketNumberAsync();
        Task<Ticket?> GetByNumberAsync(int ticketNumber);
        Task<IEnumerable<Ticket>> GetForEmployeeAsync(Guid employeeId);
        Task<IEnumerable<Ticket>> GetByTypesAsync(IReadOnlyCollection<TicketType> types, TicketStatus? status);
    }
}
