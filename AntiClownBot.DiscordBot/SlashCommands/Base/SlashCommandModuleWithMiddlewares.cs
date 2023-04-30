using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Base;

public abstract class SlashCommandModuleWithMiddlewares : ApplicationCommandModule
{
    protected SlashCommandModuleWithMiddlewares(ICommandExecutor commandExecutor)
    {
        this.commandExecutor = commandExecutor;
    }

    protected async Task ExecuteAsync(InteractionContext context, Func<Task> command)
    {
        await commandExecutor.ExecuteWithMiddlewares(context, command);
    }

    private readonly ICommandExecutor commandExecutor;
}