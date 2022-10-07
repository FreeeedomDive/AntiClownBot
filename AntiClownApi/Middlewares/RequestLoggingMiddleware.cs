using System;
using System.Threading.Tasks;
using Loggers;
using Microsoft.AspNetCore.Http;

namespace AntiClownBotApi.Middlewares;

public class RequestLoggingMiddleware
{
    public RequestLoggingMiddleware(RequestDelegate next, ILogger logger)
    {
        this.next = next;
        this.logger = logger;
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch(Exception e)
        {
            logger.Error(e, "Unhandled exception in api method {method}", context.Request?.Method);
        }
        finally
        {
            logger.Info(
                "Request {method} {url} with response status code {statusCode}",
                context.Request?.Method,
                context.Request?.Path.Value,
                context.Response?.StatusCode);
        }
    }

    private readonly ILogger logger;
    private readonly RequestDelegate next;
}