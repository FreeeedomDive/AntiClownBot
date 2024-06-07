using System.Net;
using Xdd.HttpHelpers.Models.Exceptions;
using Newtonsoft.Json;

namespace AntiClown.Web.Api.Middlewares;

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
        catch (HttpResponseExceptionBase httpResponseExceptionBase)
        {
            await WriteExceptionAsync(context, httpResponseExceptionBase, httpResponseExceptionBase.StatusCode);
        }
        catch (Exception exception)
        {
            var wrappedException = new InternalServerErrorException(exception.Message);
            await WriteExceptionAsync(context, wrappedException, wrappedException.StatusCode);
            throw;
        }
    }

    private static async Task WriteExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode)
    {
        var result = JsonConvert.SerializeObject(ex, Formatting.Indented);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(result);
    }

    private readonly RequestDelegate next;
}