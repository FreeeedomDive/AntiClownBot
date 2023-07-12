using AntiClown.Core.Dto.Exceptions;
using Newtonsoft.Json;

namespace AntiClown.Api.Middlewares;

public class ServiceExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;

    public ServiceExceptionHandlingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AntiClownBaseException antiClownApiBaseException)
        {
            await WriteExceptionAsync(context, antiClownApiBaseException, antiClownApiBaseException.StatusCode);
        }
        catch (Exception exception)
        {
            var wrappedException = new AntiClownInternalServerError(exception.Message, exception);
            await WriteExceptionAsync(context, wrappedException, wrappedException.StatusCode);
        }
    }

    private static async Task WriteExceptionAsync(HttpContext context, Exception ex, int statusCode)
    {
        var result = JsonConvert.SerializeObject(ex, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(result);
    }
}