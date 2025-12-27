using QuickPass.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPass.Application.Contracts.Identity
{
    public interface IAuthService
    {
        //Task<string> Login(LoginRequest request);
        Task<Guid> RegisterCliente(RegisterRequest request);
    }
}
