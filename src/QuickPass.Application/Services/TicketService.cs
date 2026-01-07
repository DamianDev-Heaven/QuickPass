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

  // pend
        public Task<TicketResponse?> GetByIdAsync(Guid id) => throw new NotImplementedException();
        public Task<List<TicketResponse>> GetAllAsync() => throw new NotImplementedException();
        public Task AssignTechAsync(Guid ticketId, Guid techId, Guid modifiedBy, string? comment) => throw new NotImplementedException();
        public Task ResolveAsync(Guid ticketId, Guid modifiedBy, string? comment) => throw new NotImplementedException();
        public Task CloseAsync(Guid ticketId, Guid modifiedBy, string? comment) => throw new NotImplementedException();
        public Task ReopenAsync(Guid ticketId, Guid modifiedBy, string? comment) => throw new NotImplementedException();
    }
}