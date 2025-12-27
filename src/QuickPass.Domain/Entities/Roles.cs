using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPass.Domain.Entities
{
    public class Roles
    {
        public Guid idRol {  get; set; } = Guid.NewGuid();
        public RoleNames NameRol {  get; set; }
    }
    public enum RoleNames
    {
        Administrador,Tecnico,Usuario
    }
}
