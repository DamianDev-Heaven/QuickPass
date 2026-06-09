using System;

namespace QuickPass.Domain.Entities
{
    public class TicketHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TicketId { get; set; }
        public Guid ModifiedBy { get; set; }
        public TicketStatus? PrevStatus { get; set; }
        public TicketStatus NewStatus { get; set; }
        public string? Comment { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual Ticket? Ticket { get; set; }
        public virtual Account? Modifier { get; set; }
    }
}
