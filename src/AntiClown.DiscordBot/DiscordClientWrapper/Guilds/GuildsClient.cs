using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Guilds;

public class GuildsClient : IGuildsClient
{
    public GuildsClient(
        DiscordClient discordClient,
        IAntiClownDataApiClient antiClownDataApiClient
    )
    {
        this.discordClient = discordClient;
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    public async Task<DiscordGuild> GetGuildAsync()
    {
        var guildId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "GuildId");
        var guild = await discordClient.GetGuildAsync(guildId);
        if (guild == null)
        {
            throw new ArgumentException($"Guild {guildId} doesn't exist");
        }

        return guild;
    }

    private readonly DiscordClient discordClient;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}