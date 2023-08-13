using AntiClown.DiscordBot.Releases.Domain;

namespace AntiClown.DiscordBot.Releases.Repositories;

public class CurrentReleaseProvider : ICurrentReleaseProvider
{
    public ReleaseVersion GetCurrentRelease()
    {
        return new ReleaseVersion
        {
            Major = 3,
            Minor = 1,
            Patch = 0,
            Description = "Добавлено создание ролей и присоединение к ролям",
            CreatedAt = DateTime.UtcNow,
        };
    }
}