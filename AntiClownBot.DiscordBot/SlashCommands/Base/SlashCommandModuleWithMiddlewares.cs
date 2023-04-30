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
        await commandExecutor.ExecuteWithMiddlewares(new SlashCommandContext
        {
            Context = context, Options = new SlashCommandOptions()
        }, command);
    }

    protected async Task ExecuteEphemeralAsync(InteractionContext context, Func<Task> command)
    {
        await commandExecutor.ExecuteWithMiddlewares(new SlashCommandContext
        {
            Context = context, Options = new SlashCommandOptions
            {
                IsEphemeral = true,
            }
        }, command);
    }

    private readonly ICommandExecutor commandExecutor;
}