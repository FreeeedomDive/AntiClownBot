using AntiClown.Core.Dto.Exceptions;
using Newtonsoft.Json;

namespace AntiClown.Data.Api.Middlewares;

public class ServiceExceptionHandlingMiddleware
{
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
            var wrappedException = new AntiClownInternalServerException(exception.Message);
            await WriteExceptionAsync(context, wrappedException, wrappedException.StatusCode);
            throw;
        }
    }

    private static async Task WriteExceptionAsync(HttpContext context, Exception ex, int statusCode)
    {
        var result = JsonConvert.SerializeObject(
            ex, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
            }
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(result);
    }

    private readonly RequestDelegate next;
}