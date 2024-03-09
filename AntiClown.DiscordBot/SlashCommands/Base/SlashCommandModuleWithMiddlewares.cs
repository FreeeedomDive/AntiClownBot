using AntiClown.Data.Api.Dto.Rights;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Base;

public abstract class SlashCommandModuleWithMiddlewares : ApplicationCommandModule
{
    protected SlashCommandModuleWithMiddlewares(ICommandExecutor commandExecutor)
    {
        this.commandExecutor = commandExecutor;
    }

    /// <summary>
    ///     Выполнить слэш-команду со всеми зарегистрированными миддлварками
    /// </summary>
    protected async Task ExecuteAsync(InteractionContext context, Func<Task> command, SlashCommandOptions? options = null)
    {
        options ??= new SlashCommandOptions();
        await commandExecutor.ExecuteWithMiddlewares(new SlashCommandContext
        {
            Context = context, Options = options,
        }, command);
    }

    /// <summary>
    ///     Выполнить слэш-команду со всеми зарегистрированными миддлварками, ответ на которую будет эфемерным
    /// </summary>
    protected async Task ExecuteEphemeralAsync(InteractionContext context, Func<Task> command, SlashCommandOptions? options = null)
    {
        options ??= new SlashCommandOptions();
        options.IsEphemeral = true;
        await commandExecutor.ExecuteWithMiddlewares(new SlashCommandContext
        {
            Context = context, Options = options,
        }, command);
    }

    protected async Task ExecuteWithRightsAsync(InteractionContext context, Func<Task> command, params RightsDto[] rights)
    {
        await ExecuteAsync(
            context, command, new SlashCommandOptions
            {
                RequiredRights = rights,
            }
        );
    }

    protected async Task ExecuteEphemeralWithRightsAsync(InteractionContext context, Func<Task> command, params RightsDto[] rights)
    {
        await ExecuteEphemeralAsync(
            context, command, new SlashCommandOptions
            {
                RequiredRights = rights,
            }
        );
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