using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickPass.Application.Contracts.Persistence;
using QuickPass.Infrastructure.Data;
using QuickPass.Infrastructure.Persistence;
using QuickPass.Infrastructure.Persistence.Repositories;

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
            //aqui iran mas cosas aun no como Auth o Email
            return services;
        }
    }
}
