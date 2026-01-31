using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using QuickPass.Application.Contracts.Services;
using QuickPass.Application.Services;
using System.Reflection;

namespace QuickPass.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 1. Escanea y registra todos los validadores (AbstractValidator) en este proyecto
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // 2. Registra los servicios de negocio
        services.AddScoped<ITicketService, TicketService>();

        return services;
    }
}