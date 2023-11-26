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
            Patch = 1,
            Description = "Добавлена возможность смотреть турнирую таблицу по предсказаниям за любой сезон (на момент релиза доступен только 2023)",
            CreatedAt = DateTime.UtcNow,
        };
    }
}