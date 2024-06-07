using AntiClown.Data.Api.Client;
using AntiClown.Web.Api.Attributes;

namespace AntiClown.Web.Api.Middlewares;

public class TokenAuthenticationMiddleware
{
    public TokenAuthenticationMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAntiClownDataApiClient antiClownDataApiClient)
    {
        var endpointMetadataCollection = context.GetEndpoint()!.Metadata;
        var isRequireAuth = endpointMetadataCollection.OfType<RequireUserTokenAttribute>().FirstOrDefault() is not null;
        if (!isRequireAuth)
        {
            await next(context);
            return;
        }

        context.Request.Cookies.TryGetValue("userId", out var userId);
        context.Request.Cookies.TryGetValue("token", out var token);
        await antiClownDataApiClient.Tokens.ValidateAsync(Guid.TryParse(userId, out var x) ? x : Guid.Empty, token!);
        await next(context);
    }

    private readonly RequestDelegate next;
}