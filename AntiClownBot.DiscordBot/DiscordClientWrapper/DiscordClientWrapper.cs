using AntiClownDiscordBotVersion2.DiscordClientWrapper.Emotes;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Guilds;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Members;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Messages;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Roles;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.Voice;
using AntiClownDiscordBotVersion2.Log;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper;

public class DiscordClientWrapper : IDiscordClientWrapper
{
    public DiscordClientWrapper(
        DiscordClient discordClient,
        IGuildSettingsService guildSettingsService,
        ILogger logger
    )
    {
        this.discordClient = discordClient;
        Emotes = new EmotesClient(discordClient, guildSettingsService, logger);
        Guilds = new GuildsClient(discordClient, guildSettingsService, logger);
        Members = new MembersClient(discordClient, guildSettingsService, logger);
        Messages = new MessagesClient(discordClient, guildSettingsService, logger);
        Roles = new RolesClient(discordClient, guildSettingsService, Guilds, Members, logger);
        Voice = new VoiceClient(discordClient, Guilds, guildSettingsService, logger);
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
    public IVoiceClient Voice { get; }

    private readonly DiscordClient discordClient;
}