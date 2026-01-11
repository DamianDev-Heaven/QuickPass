using QuickPass.Application.Contracts.Persistence;
using QuickPass.Application.Contracts.Services;
using QuickPass.Application.DTOs.Tickets;
using QuickPass.Domain.Entities;

namespace QuickPass.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repo;

        public TicketService(ITicketRepository repo)
        {
            _repo = repo;
        }

        public async Task<TicketResponse> CreateAsync(CreateTRequest request, Guid customerId)
        {
            var ticket = new Ticket
            {
                TicketsId = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description ?? string.Empty,
                CustomerId = customerId,
                Status = TicketStatus.Abierto,
                TechId = null
            };

            var saved = await _repo.AddAsync(ticket);

            return new TicketResponse
            {
                Id = saved.TicketsId,
                Title = saved.Title,
                Description = saved.Description,
                Status = saved.Status.ToString(),
                CustomerId = saved.CustomerId,
                TechId = saved.TechId
            };
        }

        public async Task<List<TicketResponse>> GetMineAsync(Guid customerId)
        {
            var tickets = await _repo.GetMineAsync(customerId);

            return tickets.Select(t => new TicketResponse
            {
                Id = t.TicketsId,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                CustomerId = t.CustomerId,
                TechId = t.TechId
            }).ToList();
        }

        public async Task<TicketResponse?> GetByIdAsync(Guid id)
        {
            var ticket = await _repo.GetByIdAsync(id);
            if (ticket != null)
            {
                return new TicketResponse
                {
                    Id = ticket.TicketsId,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status.ToString(),
                    CustomerId = ticket.CustomerId,
                    TechId = ticket.TechId
                };
            }
            return null;
        }
        public async Task<List<TicketResponse>> GetAllAsync() 
        {
            var tickets = await _repo.GetAllAsync();
            return tickets.Select(t => new TicketResponse
            {
                Id = t.TicketsId,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                CustomerId = t.CustomerId,
                TechId = t.TechId
            }).ToList();
        }
        public async Task AssignTechAsync(Guid ticketId, Guid techId, Guid modifiedBy, string? comment)
        {
            var ticket = await _repo.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new InvalidOperationException($"Ticket con ID {ticketId} no encontrado");

            if (ticket.Status != TicketStatus.Abierto)
                throw new InvalidOperationException($"No se puede asignar un ticket en estado {ticket.Status}");

            if (techId != modifiedBy)
            { } // Pendiente
            await _repo.AssignTechAsync(ticketId, techId, modifiedBy, comment);
        }
        public async Task<List<TicketResponse>> GetAssignedAsync(Guid techId)
        {
            var tickets = await _repo.GetAssignedAsync(techId);
            return tickets.Select(t => new TicketResponse
            {
                Id = t.TicketsId,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                CustomerId = t.CustomerId,
                TechId = t.TechId
            }).ToList();
        }
        public async Task ResolveAsync(Guid ticketId, Guid modifiedBy, string? comment)
        {
            var ticket = await _repo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new InvalidOperationException($"Ticket con ID {ticketId} no encontrado");
            await _repo.ResolveAsync(ticketId, modifiedBy, comment);
        }
        public async Task CloseAsync(Guid ticketId, Guid modifiedBy, string? comment)
        {
            var ticket = await _repo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new InvalidOperationException($"Ticket con ID {ticketId} no puede cerrarse");
            await _repo.CloseAsync(ticketId, modifiedBy, comment);
        }
        public async Task ReopenAsync(Guid ticketId, Guid modifiedBy, string? comment)
        {
            var ticket = await _repo.GetByIdAsync(ticketId);
            if (ticket == null)
                throw new InvalidOperationException($"Ticket con ID {ticketId} no puede cerrarse");
            await _repo.ReopenAsync(ticketId, modifiedBy, comment);
        }
    }
}