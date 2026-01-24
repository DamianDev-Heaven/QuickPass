using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPass.Application.DTOs.Tickets
{
    public class TicketResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; }
        public string Category { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? TechId { get; set; }
    }
}
