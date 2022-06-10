using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.Guilds;

public interface IGuildsClient
{
    Task<DiscordGuild> GetGuildAsync();
    Task<DiscordChannel> FindDiscordChannel(ulong channelId);
    Task<DiscordChannel[]> GetGuildChannels();
}