using QuickPass.Application.DTOs.Tickets;

namespace QuickPass.Application.Contracts.Services
{
    public interface ITicketService
    {
        // CREATE
        Task<TicketResponse> CreateAsync(CreateTRequest request, Guid customerId);

        // READ
        Task<TicketResponse?> GetByIdAsync(Guid id);
        Task<List<TicketResponse>> GetMineAsync(Guid customerId);
        Task<List<TicketResponse>> GetAllAsync();

        // UPDATE
        Task AssignTechAsync(Guid ticketId, Guid techId, Guid modifiedBy, string? comment);
        Task ResolveAsync(Guid ticketId, Guid modifiedBy, string? comment);
        Task CloseAsync(Guid ticketId, Guid modifiedBy, string? comment);
        Task ReopenAsync(Guid ticketId, Guid modifiedBy, string? comment);
    }
}