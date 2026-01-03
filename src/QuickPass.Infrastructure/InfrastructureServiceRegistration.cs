using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using QuickPass.Application.Contracts.Identity;
using QuickPass.Application.Contracts.Persistence;
using QuickPass.Application.DTOs;
using QuickPass.Domain.Entities;
using QuickPass.Infrastructure.Data;
using QuickPass.Infrastructure.Identity;
using QuickPass.Infrastructure.Persistence;
using QuickPass.Infrastructure.Persistence.Repositories;
using System.Text;

namespace QuickPass.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            var con = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(con, ServerVersion.AutoDetect(con)));
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
            services.AddScoped<IAuthService, AuthService>();
            // Configurar JWT desde User Secrets
            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });

            return services;
        }
    }
}