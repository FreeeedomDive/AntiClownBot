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
            Patch = 0,
            Description = "Добавлена статистика по прошедшим предсказаниям в Ф1\n"
                          + "1. Самые выбираемые гонщики среди всех пользователей\n"
                          + "2. Самые выбираемые гонщики у конкретного пользователя\n"
                          + "3. Статистика по гонщикам, кто сколько очков мог принести за весь сезон, кто чаще всех был на 10 месте и кто вылетал первым",
            CreatedAt = DateTime.UtcNow,
        };
    }
}