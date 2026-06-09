using System;

namespace QuickPass.Domain.Entities
{
    public class Role
    {
        public Guid IdRol { get; set; } = Guid.NewGuid();
        public RoleNames NameRol { get; set; }
    }

    public enum RoleNames
    {
        Administrador,
        Tecnico,
        Usuario
    }
}
