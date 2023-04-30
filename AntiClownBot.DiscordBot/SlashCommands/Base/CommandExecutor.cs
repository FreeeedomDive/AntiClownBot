using AntiClownDiscordBotVersion2.SlashCommands.Base.Middlewares;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;

namespace AntiClownDiscordBotVersion2.SlashCommands.Base;

public class CommandExecutor : ICommandExecutor
{
    public CommandExecutor(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        middlewares = new Stack<ICommandMiddleware>();
    }

    public void AddMiddleware<T>() where T : ICommandMiddleware
    {
        var middleware = serviceProvider.GetService<T>()
                         ?? throw new ArgumentException($"{typeof(T)} was not provided in DI-container");
        middlewares.Push(middleware);
    }

    public async Task ExecuteWithMiddlewares(
        SlashCommandContext interactionContext,
        Func<Task> command
    )
    {
        Func<SlashCommandContext, Task> initialCommandAction = _ => command();
        var commandWithMiddlewaresActionComposition = middlewares
            .Aggregate(
                initialCommandAction,
                (current, middleware) => context => middleware.ExecuteAsync(context, current)
            );
        await commandWithMiddlewaresActionComposition(interactionContext);
    }

    private readonly IServiceProvider serviceProvider;
    private readonly Stack<ICommandMiddleware> middlewares;
}