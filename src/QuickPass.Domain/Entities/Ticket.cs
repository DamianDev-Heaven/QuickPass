using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPass.Domain.Entities
{
    public class Ticket
    {
        public Guid TicketsId { get; set; } = Guid.NewGuid();
        public string title { get; set; } = string.Empty;
        public string description { get; set; }
        public TicketStatus status { get; set; } = TicketStatus.Abierto;
        public Guid CustomerId { get; set; }
        public Guid? TechId { get; set; }
    }

    public enum TicketStatus
    {
        Abierto,Asignado,Enproceso,Resuelto,Cerrado
    }
}
