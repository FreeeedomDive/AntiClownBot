﻿using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using DSharpPlus;
using DSharpPlus.SlashCommands;

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

    public async Task ExecuteAsync(InteractionContext context, Func<InteractionContext, Task> next)
    {
        await discordClientWrapper.Messages.RespondAsync(
            context,
            null,
            InteractionResponseType.DeferredChannelMessageWithSource,
            isEphemeral: true
        );
        await next(context);
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
}