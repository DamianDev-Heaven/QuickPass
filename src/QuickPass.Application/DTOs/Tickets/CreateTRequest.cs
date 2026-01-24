using QuickPass.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPass.Application.DTOs.Tickets
{
    public class CreateTRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public TicketPriority Priority { get; set; } = TicketPriority.Media;
        public TicketCategory Category { get; set; } = TicketCategory.General;
    }
}
