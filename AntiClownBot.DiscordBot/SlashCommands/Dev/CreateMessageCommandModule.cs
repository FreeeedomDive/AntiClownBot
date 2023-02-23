using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Dev;

public class CreateMessageCommandModule : ApplicationCommandModule
{
    public CreateMessageCommandModule(IDiscordClientWrapper discordClientWrapper)
    {
        this.discordClientWrapper = discordClientWrapper;
    }

    [SlashCommand("message", "Отправить сообщение от лица бота")]
    public async Task CreateMessage(
        InteractionContext context,
        [Option("channel", "Текстовый канал")] DiscordChannel channel,
        [Option("message", "Текст сообщения")] string message
    )
    {
        await discordClientWrapper.Messages.SendAsync(channel.Id, message);
        await discordClientWrapper.Messages.RespondAsync(context, "done");
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
}