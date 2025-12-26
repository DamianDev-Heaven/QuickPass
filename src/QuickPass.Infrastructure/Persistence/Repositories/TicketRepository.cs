using Microsoft.EntityFrameworkCore;
using QuickPass.Application.Contracts.Persistence;
using QuickPass.Domain.Entities;
using QuickPass.Infrastructure.Data;

namespace QuickPass.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _appDbContext;
        public TicketRepository(AppDbContext context)
        {
            _appDbContext = context;
        }
        public async Task<Ticket> AddAsync(Ticket ticket)
        {
            _appDbContext.tickets.Add(ticket);
            await _appDbContext.SaveChangesAsync();
            return ticket;
        }
        public async Task<Ticket?> GetByIdAsync(Guid id)
        {
            return await _appDbContext.tickets.FirstOrDefaultAsync(t => t.TicketsId == id);
        }
    }
}
