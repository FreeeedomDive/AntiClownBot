using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Base.Middlewares;

public interface ICommandMiddleware
{
    Task ExecuteAsync(InteractionContext context, Func<InteractionContext, Task> next);
}