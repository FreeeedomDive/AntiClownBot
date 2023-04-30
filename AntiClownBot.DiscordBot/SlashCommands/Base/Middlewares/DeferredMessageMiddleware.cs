using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.SlashCommands.Base.Middlewares;

/// <summary>
///     Миддлварка показывает "bot is thinking..." в самом начале выполнения команды.
///     Важно!!! В новых командах необходимо использовать редактирование ответа к интерактивности вместо создания нового ответа!
/// </summary>
public class DeferredMessageMiddleware : ICommandMiddleware
{
    public DeferredMessageMiddleware(IDiscordClientWrapper discordClientWrapper)
    {
        this.discordClientWrapper = discordClientWrapper;
    }

    public async Task ExecuteAsync(SlashCommandContext context, Func<SlashCommandContext, Task> next)
    {
        await context.Context.CreateResponseAsync(
            InteractionResponseType.DeferredChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AsEphemeral(context.Options.IsEphemeral)
        );
        await next(context);
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
}