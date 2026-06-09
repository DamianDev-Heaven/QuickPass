using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuickPass.Domain.Exceptions;
using QuickPass.Application.Exceptions;

namespace QuickPass.Infrastructure.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error no manejado: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        if (exception is NotFoundException notFoundEx)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            problemDetails.Title = "Recurso no encontrado";
            problemDetails.Detail = notFoundEx.Message;
            problemDetails.Status = StatusCodes.Status404NotFound;
        }
        else if (exception is ValidationException validationEx)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Error de validación";
            problemDetails.Detail = "Uno o más campos no cumplen las reglas.";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Extensions.Add("errors", validationEx.Errors);
        }
        else if (exception is UnauthorizedAccessException unauthorizedEx)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            problemDetails.Title = "Acceso denegado";
            problemDetails.Detail = unauthorizedEx.Message;
            problemDetails.Status = StatusCodes.Status403Forbidden;
        }
        else if (exception is InvalidOperationException invalidOpEx)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Operación no válida";
            problemDetails.Detail = invalidOpEx.Message;
            problemDetails.Status = StatusCodes.Status400BadRequest;
        }
        else
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            problemDetails.Title = "Error interno del servidor";
            problemDetails.Detail = "Ocurrió un error inesperado.";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}