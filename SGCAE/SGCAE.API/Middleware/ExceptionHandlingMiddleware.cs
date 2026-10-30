using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.Common.Exceptions;
using SGCAE.Domain.Common;

namespace SGCAE.API.Middleware;

/// <summary>Convierte las excepciones del negocio en respuestas HTTP claras (400, 404, 409, 500).</summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        var problem = new ProblemDetails();

        if (ex is ValidationException validation)
        {
            problem.Status = StatusCodes.Status400BadRequest;
            problem.Title = "Error de validación";
            problem.Detail = "Uno o más campos no son válidos.";
            problem.Extensions["errors"] = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }
        else if (ex is DomainException)
        {
            problem.Status = StatusCodes.Status400BadRequest;
            problem.Title = "Regla de negocio incumplida";
            problem.Detail = ex.Message;
        }
        else if (ex is NotFoundException)
        {
            problem.Status = StatusCodes.Status404NotFound;
            problem.Title = "Recurso no encontrado";
            problem.Detail = ex.Message;
        }
        else if (ex is ConflictException)
        {
            problem.Status = StatusCodes.Status409Conflict;
            problem.Title = "Conflicto";
            problem.Detail = ex.Message;
        }
        else
        {
            logger.LogError(ex, "Error no controlado");
            problem.Status = StatusCodes.Status500InternalServerError;
            problem.Title = "Error interno del servidor";
            problem.Detail = "Ocurrió un error inesperado.";
        }

        context.Response.StatusCode = problem.Status!.Value;
        await context.Response.WriteAsJsonAsync(problem);
    }
}
