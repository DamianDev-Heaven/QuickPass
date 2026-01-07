using System;

namespace QuickPass.Application.DTOs.Tickets
{
    public class TicketHistoryResponse
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid ModifiedBy { get; set; }
        public string? PrevStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? Comment { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
