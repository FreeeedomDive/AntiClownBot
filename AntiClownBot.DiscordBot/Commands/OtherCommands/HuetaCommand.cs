using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.Commands.OtherCommands;

public class HuetaCommand : ICommand
{
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly DiscordClient discordClient;

    public HuetaCommand(
        IDiscordClientWrapper discordClientWrapper,
        DiscordClient discordClient
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.discordClient = discordClient;
    }

    public async Task Execute(MessageCreateEventArgs e)
    {
        var slashCommands = discordClient.GetSlashCommands();
        var result = slashCommands.RegisteredCommands.Select(kv => $"{string.Join("\t", kv.Value)}");

        await discordClientWrapper.Messages.RespondAsync(e.Message, string.Join("\n", result));
    }

    public Task<string> Help()
    {
        return Task.FromResult("Полная хуета");
    }

    public string Name => "hueta";
}