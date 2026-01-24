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
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; }
        public TicketStatus Status { get; set; } = TicketStatus.Abierto;
        public Guid CustomerId { get; set; }
        public Guid? TechId { get; set; }
        public TicketPriority Priority { get; set; } = TicketPriority.Media;
        public TicketCategory Category { get; set; } = TicketCategory.General;
    }

    public enum TicketStatus
    {
        Abierto,Asignado,Enproceso,Resuelto,Cerrado
    }
    public enum TicketPriority
    {
        Baja,
        Media,
        Alta,
        Critica
    }

    public enum TicketCategory
    {
        General,
        Hardware,
        Software,
        Redes,
        Acceso
    }

}
