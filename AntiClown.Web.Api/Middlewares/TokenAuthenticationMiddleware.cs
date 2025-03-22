using AntiClown.Api.Client;
using AntiClown.Api.Dto.Users;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Dto.Tokens;
using AntiClown.Web.Api.Attributes;
using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Web.Api.Middlewares;

public class TokenAuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAntiClownApiClient antiClownApiClient, IAntiClownDataApiClient antiClownDataApiClient)
    {
        var endpointMetadataCollection = context.GetEndpoint()!.Metadata;
        var isRequireAuth = endpointMetadataCollection.OfType<RequireUserTokenAttribute>().FirstOrDefault() is not null;
        if (!isRequireAuth)
        {
            await next(context);
            return;
        }

        context.Request.Cookies.TryGetValue("telegramUserId", out var telegramUserIdString);
        context.Request.Cookies.TryGetValue("userId", out var userId);
        context.Request.Cookies.TryGetValue("token", out var token);
        if (!string.IsNullOrEmpty(telegramUserIdString) && long.TryParse(telegramUserIdString, out var telegramUserId))
        {
            var user = await antiClownApiClient.Users.FindAsync(
                new UserFilterDto
                {
                    TelegramId = telegramUserId,
                }
            );
            if (user.FirstOrDefault() is null)
            {
                throw new UnauthorizedException($"User with telegram id {telegramUserIdString} not found in AntiClown");
            }

            await next(context);
            return;
        }

        await antiClownDataApiClient.Tokens.ValidateAsync(
            Guid.TryParse(userId, out var x) ? x : Guid.Empty,
            new TokenDto
            {
                Token = token!,
            }
        );
        await next(context);
    }
}