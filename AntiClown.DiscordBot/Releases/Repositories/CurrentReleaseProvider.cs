using AntiClown.DiscordBot.Releases.Domain;

namespace AntiClown.DiscordBot.Releases.Repositories;

public class CurrentReleaseProvider : ICurrentReleaseProvider
{
    public ReleaseVersion GetCurrentRelease()
    {
        return new ReleaseVersion
        {
            Major = 3,
            Minor = 3,
            Patch = 2,
            Description = "Обновлен список предсказаний на текущую гонку",
            CreatedAt = DateTime.UtcNow,
        };
    }
}