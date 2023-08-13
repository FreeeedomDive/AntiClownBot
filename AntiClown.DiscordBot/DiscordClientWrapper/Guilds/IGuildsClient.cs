using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.DiscordClientWrapper.Guilds;

public interface IGuildsClient
{
    Task<DiscordGuild> GetGuildAsync();
}