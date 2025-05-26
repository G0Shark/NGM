using Microsoft.AspNetCore.Mvc;

namespace NGM_Server_rebuild.API.API_v1.MIddlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NGMServiceExeption ex)
        {
            await ex.Res.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "Ошибка сервера." });
        }
    }
}