using QuickPass.Application.DTOs.Auth;
namespace QuickPass.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<string> Login(LoginRequest request);
        Task<Guid> RegisterCliente(RegisterRequest request);
    }
}
