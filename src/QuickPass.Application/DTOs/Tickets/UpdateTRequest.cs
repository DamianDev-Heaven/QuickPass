using QuickPass.Domain.Entities;

namespace QuickPass.Application.DTOs.Tickets
{
    public class UpdateTRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketCategory Category { get; set; }
    }
}
