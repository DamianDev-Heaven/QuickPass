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
                return newUser.AccId; // P
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
            //Pendiente
            throw new NotImplementedException();
        }
    }
}
