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
            Patch = 2,
            Description = "Добавлены команды для просмотра информации о гонщиках\n(текущий уровень прокачки и очки в личном зачете)",
            CreatedAt = DateTime.UtcNow,
        };
    }
}