using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus;
using DSharpPlus.Entities;
using TelemetryApp.Api.Client.Log;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Guilds;

public class GuildsClient : IGuildsClient
{
    public GuildsClient(
        DiscordClient discordClient,
        IGuildSettingsService guildSettingsService
    )
    {
        this.discordClient = discordClient;
        this.guildSettingsService = guildSettingsService;
    }

    public async Task<DiscordGuild> GetGuildAsync()
    {
        var guildId = guildSettingsService.GetGuildSettings().GuildId;
        var guild = await discordClient.GetGuildAsync(guildId);
        if (guild == null)
        {
            throw new ArgumentException($"Guild {guildId} doesn't exist");
        }

        return guild;
    }

    public async Task<DiscordChannel> FindDiscordChannel(ulong channelId)
    {
        var guild = await GetGuildAsync();
        if (!guild.Channels.ContainsKey(channelId))
        {
            throw new ArgumentException($"Channel {channelId} doesn't exist");
        }
        return guild.Channels[channelId];
    }

    public async Task<DiscordChannel[]> GetGuildChannels()
    {
        var guild = await GetGuildAsync();

        return guild.Channels.Values.ToArray();
    }

    private readonly DiscordClient discordClient;
    private readonly IGuildSettingsService guildSettingsService;
}