using Microsoft.EntityFrameworkCore;
using QuickPass.Application.Contracts.Identity;
using QuickPass.Application.DTOs.Auth;
using QuickPass.Domain.Entities;
using QuickPass.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;


namespace QuickPass.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IPasswordHasher<Account> _passwordHasher;
        public AuthService (AppDbContext context, IPasswordHasher<Account> passwordHasher)
        {
            _appDbContext = context;
            _passwordHasher = passwordHasher;
        }
        public async Task<Guid> RegisterCliente(RegisterRequest request)
        {
            var RolUser = await _appDbContext.roles.FirstAsync(r => r.NameRol == RoleNames.Usuario);
            using var transaction = _appDbContext.Database.BeginTransaction();
            try
            {
                // crear 1ro la Acc
                var newAccount = new Account
                {
                    Email = request.Email,
                    RolId = RolUser.idRol
                };
                //Hashing de la contraseña
                newAccount.Pass = _passwordHasher.HashPassword(newAccount, request.Pass);
                _appDbContext.account.Add(newAccount);
                await _appDbContext.SaveChangesAsync();
                var newUser = new Users
                {
                    NameUser = request.NameUser,
                    AccId = newAccount.accId // P
                };
                _appDbContext.users.Add(newUser);
                await _appDbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return newUser.UserId; // P
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            throw new NotImplementedException();
        }
        public async Task<string> Login (LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Pass)) throw new ArgumentException($"Email o Contraseña no pueden ir vacias");
                var acc = await _appDbContext.account.Include(p => p.Role).SingleOrDefaultAsync(p => p.Email == request.Email.ToLower());
                if (acc == null) throw new ArgumentException($"Revisa Correo o contraseña");
                var result = _passwordHasher.VerifyHashedPassword(acc, acc.Pass, acc.Pass);
                if (result == PasswordVerificationResult.Failed) throw new ArgumentException("Revisa correo o contraseña");t
            } catch (Exception e)
            {
                throw new ArgumentException(e.Message, e);
            }
            throw new NotImplementedException();
        }
        public static string Gen (string user)
        {
            return "";
        }
    }
}
