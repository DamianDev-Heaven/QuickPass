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
            
            var history = new TicketHistory
            {
                TicketId = ticket.TicketsId,
                ModifiedBy = ticket.CustomerId,
                PrevStatus = null,
                NewStatus = TicketStatus.Abierto,
                Comment = "Ticket creado.",
                ChangedAt = DateTime.UtcNow
            };
            _appDbContext.ticketHistories.Add(history);

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
            
            var prevStatus = ticket.Status;
            ticket.TechId = techAccountId;
            ticket.Status = TicketStatus.Asignado;

            var history = new TicketHistory
            {
                TicketId = ticketId,
                ModifiedBy = modifiedBy,
                PrevStatus = prevStatus,
                NewStatus = ticket.Status,
                Comment = comment ?? "Técnico asignado.",
                ChangedAt = DateTime.UtcNow
            };
            _appDbContext.ticketHistories.Add(history);

            _appDbContext.tickets.Update(ticket);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task UnAssignTech(Guid ticketId, Guid techId, Guid modifiedBy, string? comment)
        {
            var ticket = await GetByIdAsync(ticketId);
            if (ticket == null) throw new InvalidOperationException($"Ticket con ID {ticketId} no encontrado");
            
            var prevStatus = ticket.Status;
            ticket.TechId = null;
            ticket.Status = TicketStatus.Abierto;

            var history = new TicketHistory
            {
                TicketId = ticketId,
                ModifiedBy = modifiedBy,
                PrevStatus = prevStatus,
                NewStatus = ticket.Status,
                Comment = comment ?? "Técnico desasignado.",
                ChangedAt = DateTime.UtcNow
            };
            _appDbContext.ticketHistories.Add(history);

            _appDbContext.tickets.Update(ticket);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task ResolveAsync(Guid ticketId, Guid modifiedBy, string? comment)
        {
            var ticket = await GetByIdAsync(ticketId);
            if (ticket != null)
            {
                var prevStatus = ticket.Status;
                ticket.Status = TicketStatus.Resuelto;

                var history = new TicketHistory
                {
                    TicketId = ticketId,
                    ModifiedBy = modifiedBy,
                    PrevStatus = prevStatus,
                    NewStatus = ticket.Status,
                    Comment = comment ?? "Ticket resuelto.",
                    ChangedAt = DateTime.UtcNow
                };
                _appDbContext.ticketHistories.Add(history);

                _appDbContext.tickets.Update(ticket);
                await _appDbContext.SaveChangesAsync();
            }
        }
        public async Task CloseAsync(Guid ticketId, Guid modifiedBy, string? comment)
        {
            var ticket = await GetByIdAsync(ticketId);
            if (ticket != null)
            {
                var prevStatus = ticket.Status;
                ticket.Status = TicketStatus.Cerrado;

                var history = new TicketHistory
                {
                    TicketId = ticketId,
                    ModifiedBy = modifiedBy,
                    PrevStatus = prevStatus,
                    NewStatus = ticket.Status,
                    Comment = comment ?? "Ticket cerrado.",
                    ChangedAt = DateTime.UtcNow
                };
                _appDbContext.ticketHistories.Add(history);

                _appDbContext.tickets.Update(ticket);
                await _appDbContext.SaveChangesAsync();
            }
        }
        public async Task ReopenAsync(Guid ticketId, Guid modifiedBy, string? comment)
        {
            var ticket = await GetByIdAsync(ticketId);
            if (ticket != null)
            {
                var prevStatus = ticket.Status;
                ticket.Status = TicketStatus.Abierto;

                var history = new TicketHistory
                {
                    TicketId = ticketId,
                    ModifiedBy = modifiedBy,
                    PrevStatus = prevStatus,
                    NewStatus = ticket.Status,
                    Comment = comment ?? "Ticket reabierto.",
                    ChangedAt = DateTime.UtcNow
                };
                _appDbContext.ticketHistories.Add(history);

                _appDbContext.tickets.Update(ticket);
                await _appDbContext.SaveChangesAsync();
            }
        }
        public async Task<List<Ticket>> GetAssignedAsync(Guid techAccountId)
        {
            return await _appDbContext.tickets.Where(t => t.TechId == techAccountId).ToListAsync();
        }
        public async Task UpdateAsync(Ticket ticket, Guid modifiedBy, string? comment)
        {
            var history = new TicketHistory
            {
                TicketId = ticket.TicketsId,
                ModifiedBy = modifiedBy,
                PrevStatus = ticket.Status,
                NewStatus = ticket.Status,
                Comment = comment ?? "Ticket actualizado.",
                ChangedAt = DateTime.UtcNow
            };
            _appDbContext.ticketHistories.Add(history);

            _appDbContext.tickets.Update(ticket);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid ticketId)
        {
            var ticket = await GetByIdAsync(ticketId);
            if (ticket != null)
            {
                _appDbContext.tickets.Remove(ticket);
                await _appDbContext.SaveChangesAsync();
            }
        }
        public async Task<List<TicketHistory>> GetHistoryAsync(Guid ticketId)
        {
            return await _appDbContext.ticketHistories
                .Where(h => h.TicketId == ticketId)
                .OrderBy(h => h.ChangedAt)
                .ToListAsync();
        }
    }
}
