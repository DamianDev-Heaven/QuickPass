using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPass.Domain.Entities
{
    public class Users
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string NameUser { get; set; }
        public string Description { get; set; }
        public string UrlPic { get; set; }
        public Guid AccId { get; set; }
        public virtual Account account { get; set; }
    }
}
