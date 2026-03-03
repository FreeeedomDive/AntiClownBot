using AntiClown.DiscordBot.Releases.Domain;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Releases;

public interface IReleaseEmbedBuilder
{
    DiscordEmbed Build(ReleaseVersion releaseVersion);
}