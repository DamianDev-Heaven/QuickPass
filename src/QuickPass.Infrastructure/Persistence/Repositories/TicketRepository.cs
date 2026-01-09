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
        public async Task<List<Ticket>> GetMineAsync(Guid customerAccId)
        {
            return await _appDbContext.tickets.Where(t => t.CustomerId == customerAccId).ToListAsync();
        }
        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _appDbContext.tickets.ToListAsync();
        }
        public async Task AssignTechAsync(Guid ticketId, Guid techAccountId, Guid modifiedBy, string? comment)
        {
            var ticket = await GetByIdAsync(ticketId);
            if (ticket == null)
                throw new InvalidOperationException($"Ticket con ID {ticketId} no encontrado");
            ticket.TechId = techAccountId;
            ticket.Status = TicketStatus.Asignado;
            _appDbContext.tickets.Update(ticket);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task ResolveAsync(Guid ticketId, Guid modifiedBy, string? comment)
        {
            var ticket = await GetByIdAsync(ticketId);
            if (ticket != null)
            {
                ticket.Status = TicketStatus.Resuelto;
                _appDbContext.tickets.Update(ticket);
                await _appDbContext.SaveChangesAsync();
            }
        }
        public async Task CloseAsync(Guid ticketId, Guid modifiedBy, string? comment)
        {
            var ticket = await GetByIdAsync(ticketId);
            if (ticket != null)
            {
                ticket.Status = TicketStatus.Cerrado;
                _appDbContext.tickets.Update(ticket);
                await _appDbContext.SaveChangesAsync();
            }
        }
        public async Task ReopenAsync(Guid ticketId, Guid modifiedBy, string? comment)
        {
            var ticket = await GetByIdAsync(ticketId);
            if (ticket != null)
            {
                ticket.Status = TicketStatus.Abierto;
                _appDbContext.tickets.Update(ticket);
                await _appDbContext.SaveChangesAsync();
            }
        }
        
    }
}
