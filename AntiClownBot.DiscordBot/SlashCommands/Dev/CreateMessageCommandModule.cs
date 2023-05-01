using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Dev;

public class CreateMessageCommandModule : SlashCommandModuleWithMiddlewares
{
    public CreateMessageCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper
    ) : base(commandExecutor)
    {
        this.discordClientWrapper = discordClientWrapper;
    }

    [SlashCommand("message", "Отправить сообщение от лица бота", false)]
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