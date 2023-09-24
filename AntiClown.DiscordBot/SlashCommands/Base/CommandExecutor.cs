using AntiClown.DiscordBot.SlashCommands.Base.Middlewares;

namespace AntiClown.DiscordBot.SlashCommands.Base;

public class CommandExecutor : ICommandExecutor
{
    public CommandExecutor(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        middlewares = new Stack<ICommandMiddleware>();
    }

    public void AddMiddleware<T>() where T : ICommandMiddleware
    {
        var middleware = serviceProvider.GetService<T>() ?? throw new ArgumentException($"{typeof(T)} was not provided in DI-container");
        middlewares.Push(middleware);
    }

    public async Task ExecuteWithMiddlewares(
        SlashCommandContext interactionContext,
        Func<Task> command
    )
    {
        var commandWithMiddlewaresActionComposition = middlewares
            .Aggregate(
                (Func<SlashCommandContext, Task>)InitialCommandAction,
                (current, middleware) => context => middleware.ExecuteAsync(context, current)
            );
        await commandWithMiddlewaresActionComposition(interactionContext);
        return;

        Task InitialCommandAction(SlashCommandContext _)
        {
            return command();
        }
    }

    private readonly Stack<ICommandMiddleware> middlewares;
    private readonly IServiceProvider serviceProvider;
}