using AntiClownDiscordBotVersion2.DiscordClientWrapper.Channels;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Emotes;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Guilds;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Members;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Messages;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Roles;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper;

public class DiscordClientWrapper : IDiscordClientWrapper
{
    public DiscordClientWrapper(
        DiscordClient discordClient,
        IGuildSettingsService guildSettingsService
    )
    {
        this.discordClient = discordClient;
        Emotes = new EmotesClient(discordClient, guildSettingsService);
        Guilds = new GuildsClient(discordClient, guildSettingsService);
        Members = new MembersClient(discordClient, guildSettingsService);
        Messages = new MessagesClient(discordClient, guildSettingsService);
        Roles = new RolesClient(discordClient, guildSettingsService, Guilds, Members);
        Channels = new ChannelsClient(discordClient, Guilds);
    }

    public async Task StartDiscord()
    {
        await discordClient.ConnectAsync();
        await Task.Delay(-1);
    }

    public IEmotesClient Emotes { get; }
    public IGuildsClient Guilds { get; }
    public IMembersClient Members { get; }
    public IMessagesClient Messages { get; }
    public IRolesClient Roles { get; }
    public IChannelsClient Channels { get; }

    private readonly DiscordClient discordClient;
}