using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Releases.Domain;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Releases;

public class ReleaseEmbedBuilder : IReleaseEmbedBuilder
{
    public DiscordEmbed Build(ReleaseVersion releaseVersion)
    {
        return new DiscordEmbedBuilder()
               .WithTitle($"Новая версия бота {releaseVersion}")
               .WithColor(DiscordColor.Green)
               .AddField("Что нового:", releaseVersion.Description)
               .WithFooter($"Время релиза: {releaseVersion.CreatedAt.ToYekaterinburgFormat()}")
               .Build();
    }
}