using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Settlr.Common.Messages;
using Settlr.Common.Response;

namespace Settlr.Web.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, AppMessages.UnhandledException);
            await WriteErrorResponseAsync(context, ex);
        }
    }

    private async Task WriteErrorResponseAsync(HttpContext context, Exception ex)
    {
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var errors = _environment.IsDevelopment() ? new[] { ex.Message } : null;
        var response = ResponseFactory.Fail<object>(AppMessages.UnhandledException, statusCode, errors);

        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
