using Settlr.Web.Middlewares;

namespace Settlr.Web.Extension;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSettlrExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static IApplicationBuilder UseSettlrSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }
}
