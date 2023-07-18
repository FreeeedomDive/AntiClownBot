using AntiClown.DiscordBot.Cache.Users;

namespace AntiClown.DiscordBot.SlashCommands.Base.Middlewares;

public class ActualizeUsersCacheMiddleware : ICommandMiddleware
{
    public ActualizeUsersCacheMiddleware(IUsersCache usersCache)
    {
        this.usersCache = usersCache;
    }

    public async Task ExecuteAsync(SlashCommandContext context, Func<SlashCommandContext, Task> next)
    {
        await usersCache.AddOrUpdate(context.Context.Member);
        await next(context);
    }

    private readonly IUsersCache usersCache;
}