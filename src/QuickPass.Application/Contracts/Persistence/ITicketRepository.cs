using QuickPass.Domain.Entities;

namespace QuickPass.Application.Contracts.Persistence
{
    public interface ITicketRepository
    {
        Task<Ticket> AddAsync(Ticket ticket);
        Task<Ticket?> GetByIdAsync (Guid id);
    }
}
