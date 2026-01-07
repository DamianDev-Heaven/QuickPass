using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPass.Application.DTOs.Tickets
{
    public class AssignTechRequest
    {
        public Guid TechId { get; set; }
        public string? Comment { get; set; }

    }
}
