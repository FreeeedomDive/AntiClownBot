using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Base;

public abstract class SlashCommandModuleWithMiddlewares : ApplicationCommandModule
{
    protected SlashCommandModuleWithMiddlewares(ICommandExecutor commandExecutor)
    {
        this.commandExecutor = commandExecutor;
    }

    /// <summary>
    ///     Выполнить слэш-команду со всеми зарегистрированными миддлварками
    /// </summary>
    protected async Task ExecuteAsync(InteractionContext context, Func<Task> command)
    {
        await commandExecutor.ExecuteWithMiddlewares(new SlashCommandContext
        {
            Context = context, Options = new SlashCommandOptions()
        }, command);
    }

    /// <summary>
    ///     Выполнить слэш-команду со всеми зарегистрированными миддлварками, ответ на которую будет эфемерным
    /// </summary>
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

    /// <summary>
    ///     Метод для корректного ответа на команду текстом
    /// </summary>
    protected static async Task<DiscordMessage> RespondToInteractionAsync(InteractionContext context, string message)
    {
        return await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent(message));
    }

    /// <summary>
    ///     Метод для корректного ответа на команду эмбедом
    /// </summary>
    protected static async Task<DiscordMessage> RespondToInteractionAsync(InteractionContext context, DiscordEmbed embed)
    {
        return await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }

    /// <summary>
    ///     Метод для корректного ответа на команду эмбедом
    /// </summary>
    protected static async Task<DiscordMessage> RespondToInteractionAsync(InteractionContext context, DiscordWebhookBuilder webhookBuilder)
    {
        return await context.EditResponseAsync(webhookBuilder);
    }

    private readonly ICommandExecutor commandExecutor;
}