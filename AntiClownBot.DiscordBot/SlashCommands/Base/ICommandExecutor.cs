using AntiClownDiscordBotVersion2.SlashCommands.Base.Middlewares;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Base;

public interface ICommandExecutor
{
    void AddMiddleware<T>() where T : ICommandMiddleware;
    Task ExecuteWithMiddlewares(InteractionContext interactionContext, Func<Task> command);
}