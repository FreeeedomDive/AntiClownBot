using AntiClown.DiscordBot.Options;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Guilds;

public class GuildsClient : IGuildsClient
{
    public GuildsClient(
        DiscordClient discordClient,
        IOptions<DiscordOptions> discordOptions
    )
    {
        this.discordClient = discordClient;
        this.discordOptions = discordOptions;
    }

    public async Task<DiscordGuild> GetGuildAsync()
    {
        var guildId = discordOptions.Value.GuildId;
        var guild = await discordClient.GetGuildAsync(guildId);
        if (guild == null)
        {
            throw new ArgumentException($"Guild {guildId} doesn't exist");
        }

        return guild;
    }

    private readonly DiscordClient discordClient;
    private readonly IOptions<DiscordOptions> discordOptions;
}