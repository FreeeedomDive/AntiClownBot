using AntiClown.DiscordBot.Releases.Domain;

namespace AntiClown.DiscordBot.Releases.Repositories;

public interface ICurrentReleaseProvider
{
    ReleaseVersion GetCurrentRelease();
}