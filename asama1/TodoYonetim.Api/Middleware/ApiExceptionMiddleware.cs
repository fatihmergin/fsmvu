using Microsoft.AspNetCore.Mvc;
using TodoYonetim.Api.Exceptions;

namespace TodoYonetim.Api.Middleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ApiExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppValidationException exception)
        {
            await WriteProblemAsync(context, StatusCodes.Status400BadRequest, "Geçersiz istek", exception.Message);
        }
        catch (ResourceNotFoundException exception)
        {
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, "Kayıt bulunamadı", exception.Message);
        }
        catch (ConflictException exception)
        {
            await WriteProblemAsync(context, StatusCodes.Status409Conflict, "İşlem çakışması", exception.Message);
        }
        catch (Exception)
        {
            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "Sunucu hatası", "İşlem sırasında beklenmeyen bir hata oluştu.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int status, string title, string detail)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };
        await context.Response.WriteAsJsonAsync(problem);
    }
}
