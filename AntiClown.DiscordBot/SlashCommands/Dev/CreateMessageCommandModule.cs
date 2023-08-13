using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Dev;

public class CreateMessageCommandModule : SlashCommandModuleWithMiddlewares
{
    public CreateMessageCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper
    ) : base(commandExecutor)
    {
        this.discordClientWrapper = discordClientWrapper;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_CreateMessage, "Отправить сообщение от лица бота", false)]
    [SlashCommandPermissions(Permissions.ViewAuditLog)]
    public async Task CreateMessage(
        InteractionContext context,
        [Option("channel", "Текстовый канал")] DiscordChannel channel,
        [Option("message", "Текст сообщения")] string message
    )
    {
        await ExecuteAsync(context, async () =>
        {
            await discordClientWrapper.Messages.SendAsync(channel.Id, message);
            await RespondToInteractionAsync(context, "done");
        });
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
}