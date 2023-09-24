using AntiClown.DiscordBot.Releases.Domain;

namespace AntiClown.DiscordBot.Releases.Repositories;

public class CurrentReleaseProvider : ICurrentReleaseProvider
{
    public ReleaseVersion GetCurrentRelease()
    {
        return new ReleaseVersion
        {
            Major = 3,
            Minor = 2,
            Patch = 0,
            Description = "Технический релиз\nВсе настройки вынесены в отдельный сервис для легкого редактирования",
            CreatedAt = DateTime.UtcNow,
        };
    }
}