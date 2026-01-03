using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuickPass.Application.Contracts.Identity;
using QuickPass.Application.DTOs;
using QuickPass.Application.DTOs.Auth;
using QuickPass.Domain.Entities;
using QuickPass.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuickPass.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly JwtSettings _jwtSettings;

        public AuthService(AppDbContext context, IPasswordHasher<Account> passwordHasher, IOptions<JwtSettings> jwtsettings)
        {
            _appDbContext = context;
            _passwordHasher = passwordHasher;
            _jwtSettings = jwtsettings.Value;
        }

        public async Task<Guid> RegisterCliente(RegisterRequest request)
        {
            var RolUser = await _appDbContext.roles.FirstAsync(r => r.NameRol == RoleNames.Usuario);
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                // crear 1ro la Acc
                var newAccount = new Account
                {
                    Email = request.Email.ToLower(),
                    RolId = RolUser.idRol
                };
                //Hashing de la contraseña
                newAccount.Pass = _passwordHasher.HashPassword(newAccount, request.Pass);
                _appDbContext.account.Add(newAccount);
                await _appDbContext.SaveChangesAsync();

                var newUser = new Users
                {
                    NameUser = request.NameUser,
                    AccId = newAccount.accId
                };
                _appDbContext.users.Add(newUser);
                await _appDbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return newUser.UserId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Pass))
                throw new ArgumentException($"Email o Contraseña no pueden ir vacias");

            var acc = await FindValAcc(request.Email, request.Pass);
            var user = await FindUsAcc(acc.accId);
            return GenToken(acc, user);
        }

        public async Task<Account> FindValAcc(string email, string pass)
        {
            var acc = await _appDbContext.account
                .Include(p => p.Role).SingleOrDefaultAsync(p => p.Email == email.ToLower());
            
            if (acc == null) throw new ArgumentException("Credenciales inválidas");
            if (string.IsNullOrEmpty(acc.Pass))
                throw new InvalidOperationException("Cuenta sin contraseña configurada");
            
            var result = _passwordHasher.VerifyHashedPassword(acc, acc.Pass, pass);
            if (result == PasswordVerificationResult.Failed) 
                throw new ArgumentException("Revisa correo o contraseña");
            
            return acc;
        }

        public async Task<Users> FindUsAcc(Guid accId)
        {
            var user = await _appDbContext.users.SingleOrDefaultAsync(u => u.AccId == accId);
            return user ?? throw new InvalidOperationException("Cuenta sin usuario asociado");
        }

        private string GenToken(Account acc, Users usr)
        {
            var claims = new List<Claim>
            {
                // Identidad Prin 
                new Claim(JwtRegisteredClaimNames.Sub, usr.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, usr.UserId.ToString()),
                // Datos de cuenta
                new Claim(JwtRegisteredClaimNames.Email, acc.Email),
                new Claim(ClaimTypes.Role, acc.Role!.NameRol.ToString()),
                // Datos del usuario
                new Claim("name", usr.NameUser),
                new Claim("accountId", usr.AccId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}