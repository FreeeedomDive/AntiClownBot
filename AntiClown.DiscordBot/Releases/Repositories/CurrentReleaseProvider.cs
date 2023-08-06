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
            Patch = 0,
            Description = "Добавлен эвент с гонками\nДобавлено отображение информации про новые релизы",
            CreatedAt = DateTime.UtcNow,
        };
    }
}