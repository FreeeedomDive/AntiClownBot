using AntiClown.DiscordBot.SlashCommands.Base.Middlewares;

namespace AntiClown.DiscordBot.SlashCommands.Base;

public interface ICommandExecutor
{
    void AddMiddleware<T>() where T : ICommandMiddleware;
    Task ExecuteWithMiddlewares(SlashCommandContext interactionContext, Func<Task> command);
}