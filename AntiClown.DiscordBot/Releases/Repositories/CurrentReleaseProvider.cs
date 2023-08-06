using AntiClown.DiscordBot.Releases.Domain;

namespace AntiClown.DiscordBot.Releases.Repositories;

public class CurrentReleaseProvider : ICurrentReleaseProvider
{
    public ReleaseVersion GetCurrentRelease()
    {
        return new ReleaseVersion
        {
            Major = 3,
            Minor = 0,
            Patch = 1,
            Description = "Исправлены ошибки в гонках",
            CreatedAt = DateTime.UtcNow,
        };
    }
}