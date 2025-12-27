using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPass.Domain.Entities
{
    public class Account
    {
        public Guid accId { get; set; } = Guid.NewGuid();
        public string Email {  get; set; } 
        public string Pass {  get; set; } = string.Empty;
        public bool OtherMed { get; set; }
        public Guid RolId { get; set; }
        // Navegacion
        public virtual Roles? Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
