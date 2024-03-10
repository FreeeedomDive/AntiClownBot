using AntiClown.Data.Api.Client;
using AntiClown.DiscordBot.Cache.Users;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.SlashCommands.Base.Middlewares;

public class CheckRightsMiddleware : ICommandMiddleware
{
    public CheckRightsMiddleware(
        IAntiClownDataApiClient antiClownDataApiClient,
        IUsersCache usersCache
    )
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.usersCache = usersCache;
    }

    public async Task ExecuteAsync(SlashCommandContext context, Func<SlashCommandContext, Task> next)
    {
        var commandRights = context.Options.RequiredRights;
        if (commandRights.Length == 0)
        {
            await next(context);
            return;
        }

        var apiUserId = await usersCache.GetApiIdByMemberIdAsync(context.Context.Member.Id);
        var userRights = await antiClownDataApiClient.Rights.FindAllUserRightsAsync(apiUserId);
        var hasRights = userRights.Intersect(commandRights).Any();
        if (!hasRights)
        {
            await context.Context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Недостаточно прав для выполнения этой команды"));
            return;
        }

        await next(context);
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IUsersCache usersCache;
}