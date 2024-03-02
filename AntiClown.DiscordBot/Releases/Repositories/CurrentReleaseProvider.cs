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
            Patch = 1,
            Description = "На фронте добавлен выбор гонки, на которую делается предсказание. "
                          + "В боте добавлены админские команды, чтобы вносить результаты по новым предсказаниям",
            CreatedAt = DateTime.UtcNow,
        };
    }
}