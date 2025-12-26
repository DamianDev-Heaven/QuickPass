using QuickPass.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPass.Application.Contracts.Persistence
{
    public interface IRolesRepository
    {
        Task<IReadOnlyCollection<Roles>> GetAllRoles();
    }
}
