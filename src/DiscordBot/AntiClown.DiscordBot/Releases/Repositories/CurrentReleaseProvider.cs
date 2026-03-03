using AntiClown.DiscordBot.Releases.Domain;

namespace AntiClown.DiscordBot.Releases.Repositories;

public class CurrentReleaseProvider : ICurrentReleaseProvider
{
    public ReleaseVersion GetCurrentRelease()
    {
        return new ReleaseVersion
        {
            Major = 3,
            Minor = 4,
            Patch = 2,
            Description = "Исправлена проблема с дублированием пати, если после его создания слишком быстро была нажата кнопка Присоединиться",
            CreatedAt = DateTime.UtcNow,
        };
    }
}