using QuickPass.Domain.Entities;

namespace QuickPass.Application.Contracts.Persistence
{
    public interface ITicketRepository
    {
        Task<Ticket> AddAsync(Ticket ticket);

        // READ
        Task<Ticket?> GetByIdAsync(Guid id);
        Task<List<Ticket>> GetMineAsync(Guid customerAccountId);
        Task<List<Ticket>> GetAllAsync();
        Task<List<Ticket>> GetAssignedAsync(Guid techId);

        // UPDATE
        Task AssignTechAsync(Guid ticketId, Guid techAccountId, Guid modifiedBy, string? comment);
        Task UnAssignTech(Guid ticketId, Guid techAccountId, Guid modifiedBy, string? comment);
        Task ResolveAsync(Guid ticketId, Guid modifiedBy, string? comment);
        Task CloseAsync(Guid ticketId, Guid modifiedBy, string? comment);
        Task ReopenAsync(Guid ticketId, Guid modifiedBy, string? comment);
        Task UpdateAsync(Ticket ticket, Guid modifiedBy, string? comment);
        Task DeleteAsync(Guid ticketId);

        // HISTORY
        Task<List<TicketHistory>> GetHistoryAsync(Guid ticketId);
    }
}
